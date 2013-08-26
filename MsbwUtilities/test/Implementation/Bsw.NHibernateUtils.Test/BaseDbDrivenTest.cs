// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details
﻿#region

using System;
using System.Linq;
using System.Linq.Expressions;
using Bsw.NHibernate.Testing.SqlLite;
using Bsw.NHibernateUtils.Mapping;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NUnit.Framework;

#endregion

namespace Bsw.NHibernateUtils.Test
{
    public abstract class BaseDbDrivenTest : BaseTest
    {
        private static SessionFactoryFetcher _sessionFactoryFetcher;
        protected static ISessionFactory SessionFactory { get; private set; }

        [SetUp]
        public override void SetUp()
        {
        }

        [TearDown]
        public override void TearDown()
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
                                                        .Conventions.Add(DefaultCascade.All())
                                                        .SupportNullablePrimaryKeysNativeGenerated()
                                                        .UseOverridesFromAssemblyOf<NullablePrimaryKeyConvention>()));
        }
    }
}