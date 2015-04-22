using FluentNHibernate.Cfg;
using NCore.Security.Model;
using NCore.Security.NHibernate.Model;
using NHibernate.Cfg;

namespace NCore.Security.NHibernate
{
    /// <summary>
    /// Ядро системы безопасности
    /// </summary>
    public static class SecurityCore
    {
        public static void Configure(FluentConfiguration cfg)
        {
            cfg.Mappings(x => x.FluentMappings.AddFromAssemblyOf<UserMap>());
            new UserMapper(cfg.BuildConfiguration(), typeof(User)).Map();
        }
    }
}
