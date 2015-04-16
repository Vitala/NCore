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
                .AddModule(new NHibernatePostgreModule()
                {
                    AssemblyMapper = Assembly.GetExecutingAssembly(),
                    ConnectionStringKey = "ncore-test-base",
                    AfterConfigure = e => {
                        SecurityCore.Configure(e);
           
                    }
                })
                .AddModule(new NCoreNhibernateSecurityModule())
                .Configure(c => {
                    c.RegisterType<TestService>().As<ITestService>();
                    c.RegisterType<TestRepository>().As<ITestRepository>();
                })
                .Build();

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
