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

namespace Bsw.NHibernate.Testing.SqlLite
{
    public class SessionFactoryFetcher
    {
        private const string CONNECTION_STRING = "FullUri=file:memorydb.db?mode=memory&cache=shared";

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
            var sessionFactory = fluentConfig.ExposeConfiguration(c =>
                                                                  {
                                                                      savedConfig = c;
                                                                  })
                                             .BuildSessionFactory();

            var schemaExport = new SchemaExport(savedConfig);
            Connection = new SQLiteConnection(CONNECTION_STRING);
            Connection.Open();
            schemaExport.Execute(true,
                                 true,
                                 false,
                                 Connection,
                                 null);
            return sessionFactory;
        }
    }
}