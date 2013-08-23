#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernate.Testing.SqlLite;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NUnit.Framework;

#endregion

namespace Bsw.NHibernateUtils.Test
{
    public class BaseTest
    {
        private static SessionFactoryFetcher _sessionFactoryFetcher;
        protected static ISessionFactory SessionFactory { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        [TestFixtureSetUp]
        public static void FixtureSetup()
        {
            _sessionFactoryFetcher = new SessionFactoryFetcher();
            SessionFactory = _sessionFactoryFetcher.GetSessionFactory(SetupTestMappings);
        }

        [TestFixtureTearDown]
        public static void FixtureTearDown()
        {
            _sessionFactoryFetcher.Connection.Dispose();
        }

        private static void SetupTestMappings(FluentConfiguration config)
        {
            config.Mappings(map =>
                            map.AutoMappings.Add(AutoMap.AssemblyOf<BaseTest>()
                                                        .Where(typ => typ.Namespace.EndsWith("TestEntities"))
                                                        .Conventions.Add(DefaultCascade.All())));
        }
    }
}