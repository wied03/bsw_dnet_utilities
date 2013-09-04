// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;

#endregion

namespace Bsw.NHibernateUtils.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private Lazy<ISession> _lazySession;
        private ITransaction _transaction;
        private readonly ISessionFactory _sessionFactory;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            _transaction = null;
            CreateSessionAndTransaction();
            Disposed = false;
        }

        private void CreateSessionAndTransaction()
        {
            _lazySession = new Lazy<ISession>(() =>
                                              {
                                                  var session = _sessionFactory.OpenSession();
                                                  _transaction = session.BeginTransaction();
                                                  return session;
                                              });
        }

        public ISession CurrentSession
        {
            get { return _lazySession.Value; }
        }

        public void Dispose()
        {
            // If we never actually opened a session, we don't need to do anything
            if (Disposed || !_lazySession.IsValueCreated || CurrentSession == null) return;
            if (_transaction.IsActive)
            {
                // most of the time, this will be the last unit of work for the HTTP request (designed for
                // the HTTP module. so don't open another one.
                Commit(openNewTransactionAfterCommittingCurrent: false);
            }

            CurrentSession.Close();
            _lazySession = null;
            Disposed = true;
        }

        public virtual void Commit(bool openNewTransactionAfterCommittingCurrent = true)
        {
            try
            {
                _transaction.Commit();
            }
            catch (Exception)
            {
                // avoid NHibernate complaining
                CurrentSession.Dispose();
                CreateSessionAndTransaction();
                throw;
            }
            // deal with cases where we want to commit changes then read back those changes in a 
            // subsequent transaction
            if (!openNewTransactionAfterCommittingCurrent) return;
            _transaction.Dispose();
            _transaction = CurrentSession.BeginTransaction();
        }

        public void Rollback()
        {
            if (!_transaction.WasRolledBack && _transaction.IsActive)
            {
                _transaction.Rollback();
            }
        }

        public bool Disposed { get; private set; }
    }
}