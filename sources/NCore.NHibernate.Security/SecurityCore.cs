using NCore.NHibernate.Security.Model;
using NHibernate.Cfg;

namespace NCore.NHibernate.Security
{
    /// <summary>
    /// Ядро системы безопасности
    /// </summary>
    public static class SecurityCore
    {
        public static void Configure(Configuration cfg)
        {
            cfg.AddAssembly(typeof(User).Assembly);
            new UserMapper(cfg, typeof(User)).Map();
        }
    }
}
