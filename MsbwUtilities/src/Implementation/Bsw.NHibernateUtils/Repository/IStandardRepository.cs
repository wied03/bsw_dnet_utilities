using System.Linq;
using NHibernate;

namespace Bsw.NHibernateUtils.Repository
{
    public enum TransactionActionOnError
    {
        Nothing,
        Rollback
    }

    public interface IStandardRepository<TEntityType> where TEntityType : class
    {
        /// <summary>
        /// What to do when an error occurs within one of these methods.  Default is TransactionActionOnError.Rollback
        /// </summary>
        TransactionActionOnError TransactionActionOnError { get; set; }

        IQueryable<TEntityType> Query();
        TEntityType Get(object id);
        void SaveOrUpdate(TEntityType entity);
        void Delete(TEntityType entity);
        ICriteria CreateCriteria();
        ISQLQuery CreateSqlQuery(string query);
        IQuery CreateHqlQuery(string query);
        void Save(TEntityType entity);
        void Update(TEntityType entity);
    }
}