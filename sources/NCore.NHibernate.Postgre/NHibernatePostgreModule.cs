using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NCore.Domain;
using NCore.NHibernate.Domain;
using NHibernate;
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
                            .BuildSessionFactory();
            
            builder.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
            builder.Register(c => { return c.Resolve<ISessionFactory>().OpenSession(); }).As<ISession>();
            builder.RegisterType<CallContextCurrentUnitOfWorkProvider>().As<ICurrentUnitOfWorkProvider>();
            builder.RegisterType<CurrentSessionProvider>().As<ICurrentSessionProvider>();
            builder.RegisterType<NhUnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(NhRepository<,>)).As(typeof(IRepository<,>));
        }
    }
}