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
        }

        protected ISession Session
        {
            get { return Uow.CurrentSession; }
        }

        public IQueryable<TEntityType> Query()
        {
            return Session.Query<TEntityType>();
        }

        public TEntityType Get(object id)
        {
            return Session.Get<TEntityType>(id);
        }

        public void SaveOrUpdate(TEntityType entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public void Delete(TEntityType entity)
        {
            Session.Delete(entity);
        }

        public ICriteria CreateCriteria()
        {
            return Session.CreateCriteria<TEntityType>();
        }

        public ISQLQuery CreateSqlQuery(string query)
        {
            return Session.CreateSQLQuery(query);
        }

        public IQuery CreateHqlQuery(string query)
        {
            return Session.CreateQuery(query);
        }
    }
}