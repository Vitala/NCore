using Autofac;
using NCore.Security.NHibernate.Interfaces;
using NCore.Security.NHibernate.Services;

namespace NCore.Security.NHibernate
{
    public class NCoreSecurityNHibernateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthorizationRepository>().As<IAuthorizationRepository>();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>();
            builder.RegisterType<PermissionsBuilderService>().As<IPermissionsBuilderService>();
        }
    }
}
