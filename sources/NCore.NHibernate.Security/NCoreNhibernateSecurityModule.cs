using Autofac;
using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Services;

namespace NCore.NHibernate.Security
{
    public class NCoreNhibernateSecurityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SecurityCore>().AsSelf().SingleInstance();
            builder.RegisterType<AuthorizationRepository>().As<IAuthorizationRepository>();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>();
            // builder.RegisterType<PermissionsService>().As<IPermissionsService>();
            builder.RegisterType<PermissionsBuilderService>().As<IPermissionsBuilderService>();
        }
    }
}
