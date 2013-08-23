#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernateUtils.Repository;
using NHibernate;
using StructureMap.Configuration.DSL;

#endregion

namespace Bsw.NHibernateUtils.Dependencies
{
    public abstract class BasicNHibernateRegistry : Registry
    {
        protected BasicNHibernateRegistry()
        {
        }

        protected void SetupSessionFactory(ISessionFactory sessionFactory)
        {
            For<ISessionFactory>()
                .Singleton()
                .Use(sessionFactory);
        }

        protected void SetupUnitOfWork()
        {
            For<IUnitOfWork>()
                .HybridHttpOrThreadLocalScoped()
                .Use<UnitOfWork>();
        }
    }
}