using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Context;
using System.Reflection;
using Configuration = NHibernate.Cfg.Configuration;
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
                            .CurrentSessionContext(typeof(CallSessionContext).FullName)
                            .BuildSessionFactory();

            builder.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
            builder.RegisterType<CurrentSessionProvider>().As<ICurrentSessionProvider>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));
        }
    }
}