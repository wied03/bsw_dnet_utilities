// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Bsw.NHibernateUtils.Repository
{
    public class StandardRepository : IStandardRepository
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

        private TReturnType RollbackIfNecessary<TReturnType>(Func<ISession, TReturnType> action)
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

        public IQueryable<TEntityType> Query<TEntityType>()
        {
            return RollbackIfNecessary(s => s.Query<TEntityType>());
        }

        public TEntityType Get<TEntityType>(object id)
        {
            return RollbackIfNecessary(s => s.Get<TEntityType>(id));
        }

        public void SaveOrUpdate<TEntityType>(TEntityType entity)
        {
            RollbackIfNecessary(s => s.SaveOrUpdate(entity));
        }

        public void Save<TEntityType>(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Save(entity));
        }

        public void Update<TEntityType>(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Update(entity));
        }

        public void Delete<TEntityType>(TEntityType entity)
        {
            RollbackIfNecessary(s => s.Delete(entity));
        }

        public ICriteria CreateCriteria<TEntityType>() where TEntityType : class
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