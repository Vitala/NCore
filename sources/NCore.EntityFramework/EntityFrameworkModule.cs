using Autofac;
using NCore.Domain;
using NCore.EntityFramework.Domain;
using System.Reflection;
using Module = Autofac.Module;

namespace NCore.EntityFramework.Infrastructure
{
    public class EntityFrameworkModule : Module
    {
        public string ConnectionStringKey { get; set; }
        public Assembly AssemblyMapper { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            var dbContextFactory = DbContextFactoryBuilder.Create()
                                    .WithConnectionStringName(ConnectionStringKey)
                                    .MapEntitiesFrom(AssemblyMapper)
                                    .Build();

            builder.RegisterInstance(dbContextFactory).As<IDbContextFactory>().SingleInstance();
            builder.RegisterType<CallContextCurrentUnitOfWorkProvider>().As<ICurrentUnitOfWorkProvider>();
            builder.RegisterType<CurrentDbContextProvider>().As<ICurrentDbContextProvider>();
            builder.RegisterType<EfUnitOfWork>().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(EfRepository<,>)).As(typeof(IRepository<,>));
        }

    }
}
