#region

// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

#region

using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

#endregion

#endregion

namespace Bsw.NHibernate.Testing.SqlLite
{
    public class SessionFactoryFetcher
    {
        SchemaExport _schemaExport;
        const string CONNECTION_STRING = "FullUri=file:memorydb.db?mode=memory&cache=shared";

        public IDbConnection Connection { get; private set; }

        public ISessionFactory GetSessionFactory(Action<FluentConfiguration> configChanges)
        {
            var sqLiteConfiguration = SQLiteConfiguration
                .Standard
                .ConnectionString(CONNECTION_STRING)
                .ShowSql();

            var fluentConfig = Fluently
                .Configure()
                .Database(sqLiteConfiguration)
                ;

            configChanges(fluentConfig);

            Configuration savedConfig = null;
            var sessionFactory = fluentConfig.ExposeConfiguration(c => { savedConfig = c; })
                                             .BuildSessionFactory();

            _schemaExport = new SchemaExport(savedConfig);
            Connection = new SQLiteConnection(CONNECTION_STRING);
            Connection.Open();
            return sessionFactory;
        }

        public void DropAndCreateSchema()
        {
            _schemaExport.Execute(true,
                                  true,
                                  false,
                                  Connection,
                                  null);
        }
    }
}