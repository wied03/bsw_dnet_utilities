using System.Linq;
using NHibernate;

namespace Bsw.NHibernateUtils.Repository
{
    public interface IStandardRepository<TEntityType> where TEntityType : class
    {
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