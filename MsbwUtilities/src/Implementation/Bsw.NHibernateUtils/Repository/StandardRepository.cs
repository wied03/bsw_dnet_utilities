#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Bsw.NHibernateUtils.Repository
{
    public class StandardRepository<TEntityType> : IStandardRepository<TEntityType> where TEntityType : class
    {
        protected IUnitOfWork Uow { get; private set; }

        public StandardRepository(IUnitOfWork uow)
        {
            Uow = uow;
            TransactionActionOnError = TransactionActionOnError.Rollback;
        }

        protected ISession Session
        {
            get { return Uow.CurrentSession; }
        }

        public TransactionActionOnError TransactionActionOnError { get; set; }

        private TReturnType RollbackIfNecessary<TReturnType>(Func<ISession,TReturnType> action)
        {
            try
            {
                return action(Session);
            }
            catch (Exception)
            {
                if (TransactionActionOnError == TransactionActionOnError.Rollback)
                {
                    Session.Transaction.Rollback();
                }
                throw;
            }
        }

        private void RollbackIfNecessary(Action<ISession> action)
        {
            RollbackIfNecessary(s =>
                                {
                                    action(s);
                                    return true;
                                });
        }

        public IQueryable<TEntityType> Query()
        {
            return RollbackIfNecessary(s => s.Query<TEntityType>());
        }

        public TEntityType Get(object id)
        {
            return RollbackIfNecessary(s => s.Get<TEntityType>(id));
        }

        public void SaveOrUpdate(TEntityType entity)
        {
            RollbackIfNecessary(s => s.SaveOrUpdate(entity));
        }

        public void Save(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Save(entity));
        }

        public void Update(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Update(entity));
        }

        public void Delete(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Delete(entity));
        }

        public ICriteria CreateCriteria()
        {
            return RollbackIfNecessary(s => s.CreateCriteria<TEntityType>());
        }

        public ISQLQuery CreateSqlQuery(string query)
        {
            return RollbackIfNecessary(s => s.CreateSQLQuery(query));
        }

        public IQuery CreateHqlQuery(string query)
        {
            return RollbackIfNecessary(s => s.CreateQuery(query));
        }
    }
}