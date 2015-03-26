using Autofac;
using NCore.Kernel;
using NCore.NHibernate.Postgre;
using NCore.TestApp.Services;
using System;
using System.Reflection;

namespace NCore.TestApp
{
    public class Bootstrapper : IDisposable
    {
        private AppCore _core;

        public void Start()
        {
            var builder = new ContainerBuilder();

            _core = AppCoreBuilder.Create(builder)
                .AddModule(new NHibernatePostgreModule
                {
                    ConnectionStringKey = "ncore-test-base",
                    AssemblyMapper = Assembly.GetExecutingAssembly()
                })
                .Configure(c => {
                    c.RegisterType<TestService>().As<ITestService>();
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
