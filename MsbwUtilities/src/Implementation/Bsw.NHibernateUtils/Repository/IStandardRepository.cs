// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System.Linq;
using NHibernate;

namespace Bsw.NHibernateUtils.Repository
{
    public enum TransactionActionOnError
    {
        Nothing,
        Rollback
    }

    public interface IStandardRepository
    {
        /// <summary>
        ///     What to do when an error occurs within one of these methods.  Default is TransactionActionOnError.Rollback
        /// </summary>
        TransactionActionOnError TransactionActionOnError { get; set; }

        IQueryable<TEntityType> Query<TEntityType>();
        TEntityType Get<TEntityType>(object id);
        void SaveOrUpdate<TEntityType>(TEntityType entity);
        void Delete<TEntityType>(TEntityType entity);
        ICriteria CreateCriteria<TEntityType>() where TEntityType : class;
        ISQLQuery CreateSqlQuery(string query);
        IQuery CreateHqlQuery(string query);
        void Save<TEntityType>(TEntityType entity);
        void Update<TEntityType>(TEntityType entity);
    }
}