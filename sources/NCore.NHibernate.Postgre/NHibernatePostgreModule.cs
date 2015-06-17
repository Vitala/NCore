using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NCore.Domain;
using NCore.NHibernate.Domain;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Reflection;
using Configuration = NHibernate.Cfg.Configuration;
using Module = Autofac.Module;


namespace NCore.NHibernate.Postgre
{
    public class NHibernatePostgreModule : Module
    {
        public string ConnectionStringKey { get; set; }
        public Assembly AssemblyMapper { get; set; }
        public Action<FluentConfiguration> AfterConfigure { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            var rawConfig = new Configuration();
            rawConfig.SetNamingStrategy(new PostgreSqlNamingStrategy());

            var fluentConfiguration = Fluently.Configure(rawConfig)
                          .Database(PostgreSQLConfiguration.PostgreSQL82
                          .ConnectionString(c => c.FromConnectionStringWithKey(ConnectionStringKey)))
                          .Mappings(x => x.FluentMappings.AddFromAssembly(AssemblyMapper))
                         .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true));
                        //.ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(false, true, false));

            if (AfterConfigure != null)
                AfterConfigure(fluentConfiguration);

            var sessionFactory = fluentConfiguration.BuildSessionFactory();

            builder.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
            builder.RegisterType<CallContextCurrentUnitOfWorkProvider>().As<ICurrentUnitOfWorkProvider>();
            builder.RegisterType<CurrentSessionProvider>().As<ICurrentSessionProvider>();
            builder.RegisterType<RepositoryFactory>().As<IRepositoryFactory>();
            builder.RegisterType<NhUnitOfWork>().As<IUnitOfWork>().As<IUnitOfWorkImplementation>();
            builder.RegisterGeneric(typeof(NhRepository<,>)).As(typeof(IRepository<,>));
        }
    }
}