// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;

#endregion

namespace Bsw.NHibernateUtils.Repository
{
    public class LazySessionFetcher : ILazySessionFetcher
    {
        Lazy<ISession> _lazySession;
        readonly ISessionFactory _sessionFactory;

        public LazySessionFetcher(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            CreateSessionAndTransaction();
            Disposed = false;
        }

        void CreateSessionAndTransaction()
        {
            _lazySession = new Lazy<ISession>(() =>
                                              {
                                                  var session = _sessionFactory.OpenSession();
                                                  return session;
                                              });
        }

        public ISession Session
        {
            get { return _lazySession.Value; }
        }

        public void Dispose()
        {
            // If we never actually opened a session, we don't need to do anything
            if (Disposed || !_lazySession.IsValueCreated || Session == null) return;
            Session.Dispose();
            _lazySession = null;
            Disposed = true;
        }

        public bool Disposed { get; private set; }
    }
}