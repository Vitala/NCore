using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Autofac;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Configuration = NHibernate.Cfg.Configuration;
using Environment = NHibernate.Cfg.Environment;
using Module = Autofac.Module;


namespace NCore.NHibernate.Postgre
{
    public class NHibernatePostgreModule : Module
    {
        public string ConnectionStringKey { get; set; }
        public Assembly AssemblyMapper { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            var rawConfig = new Configuration();
            rawConfig.SetNamingStrategy(new PostgreSqlNamingStrategy());

            var sessionFactory = Fluently.Configure(rawConfig)
                            .Database(PostgreSQLConfiguration.Standard
                            .ConnectionString(c => c.FromConnectionStringWithKey(ConnectionStringKey)))
                            .Mappings(x => x.FluentMappings.AddFromAssembly(AssemblyMapper))
                            .BuildSessionFactory();

            builder.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>()
                                              .As<INhUnitOfWork>()
                                              .AsSelf()
                                              .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
        }
    }
}