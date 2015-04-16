using FluentNHibernate.Cfg;
using NCore.NHibernate.Security.Model;
using NHibernate.Cfg;

namespace NCore.NHibernate.Security
{
    /// <summary>
    /// Ядро системы безопасности
    /// </summary>
    public static class SecurityCore
    {
        public static void Configure(FluentConfiguration cfg)
        {
            cfg.Mappings(x => x.FluentMappings.AddFromAssemblyOf<User>());
            new UserMapper(cfg.BuildConfiguration(), typeof(User)).Map();
        }
    }
}
