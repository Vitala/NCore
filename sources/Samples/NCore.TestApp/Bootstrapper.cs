using Autofac;
using System;
using System.Reflection;
using NCore.NHibernate.Postgre;
using NCore.TestApp.Services;
using NCore.Kernel;
using NHibernate.Cfg;
using NCore.NHibernate.Security;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NCore.NHibernate;
using NCore.NHibernate.Domain;
using NCore.Domain;
using NHibernate.Tool.hbm2ddl;
using NCore.NHibernate.Security.Model;
using NCore.TestApp.Entities;
using NCore.NHibernate.Security.Interfaces;

namespace NCore.TestApp
{
    public class Bootstrapper : IDisposable
    {
        private AppCore _core;

        public void Start()
        {
            var builder = new ContainerBuilder();

            _core = AppCoreBuilder.Create(builder)
                .AddModule(new NCoreNhibernateSecurityModule())
                .Configure(c => {
                    c.RegisterType<TestService>().As<ITestService>();

                    var rawConfig = new Configuration();
                    rawConfig.SetNamingStrategy(new PostgreSqlNamingStrategy());

                    var confg = Fluently.Configure(rawConfig)
                                    .Database(PostgreSQLConfiguration.PostgreSQL82
                                    .ConnectionString(b => b.FromConnectionStringWithKey("ncore-test-base")))
                                    .Mappings(x => { 
                                        x.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly());
                                        x.FluentMappings.AddFromAssemblyOf<User>();
                                    
                                    })
                                    .ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(true, true, false));

                    SecurityCore.Configure(confg.BuildConfiguration());

                    var sessionFactory = confg.BuildSessionFactory();
                    
                    c.RegisterInstance(sessionFactory).As<ISessionFactory>().SingleInstance();
                    c.Register(b => { return b.Resolve<ISessionFactory>().OpenSession(); }).As<ISession>();
                    c.RegisterType<CallContextCurrentUnitOfWorkProvider>().As<ICurrentUnitOfWorkProvider>();
                    c.RegisterType<CurrentSessionProvider>().As<ICurrentSessionProvider>();
                    c.RegisterType<NhUnitOfWork>().As<IUnitOfWork>();
                    c.RegisterGeneric(typeof(NhRepository<,>)).As(typeof(IRepository<,>));
                    c.RegisterType<TestEntitySecurityInformationExtractor>().As<IEntityInformationExtractor<TestEntity>>();
                    c.RegisterType<TestRepository>().As<ITestRepository>();
                })
                .Build();
            var securityCore = _core.Resolve<SecurityCore>();
            var service = _core.Resolve<ITestService>();
            service.AddTestRecord();

        }

        public void Dispose()
        {
            if (_core != null)
            {
                _core.Dispose();
            }
        }
    }
}
