using System;
using NHibernate;

namespace Bsw.NHibernateUtils.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ISession CurrentSession { get; }
        void Commit(bool openNewTransactionAfterCommittingCurrent = true);
        void Rollback();
    }
}