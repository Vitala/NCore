using Autofac;
using FluentNHibernate.Cfg;
using NCore.Kernel;
using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NCore.NHibernate.Security
{
    /// <summary>
    /// Ядро системы безопасности
    /// </summary>
    public class SecurityCore
    {
        private static readonly MethodInfo getSecurityKeyMethod = typeof(SecurityCore).GetMethod(
            "GetSecurityKeyPropertyInternal", BindingFlags.NonPublic | BindingFlags.Static);

        private static readonly Dictionary<Type, Func<string>> GetSecurityKeyPropertyCache =
            new Dictionary<Type, Func<string>>();

        public static SecurityCore Instance { get; private set; }

        public SecurityCore()
        {
            Instance = this;
        }

        public Guid ExtractKey<TEntity>(TEntity entity)
            where TEntity : class
        {
            var extractor = AppCore.Instance.Resolve<IEntityInformationExtractor<TEntity>>();
            return extractor.GetSecurityKeyFor(entity);
        }

        public string GetDescription<TEntity>(TEntity entity) where TEntity : class
        {
            IEntityInformationExtractor<TEntity> extractor = AppCore.Instance.Resolve<IEntityInformationExtractor<TEntity>>();
            return extractor.GetDescription(ExtractKey(entity));
        }

        public static string GetSecurityKeyProperty(Type entityType)
        {
            lock (GetSecurityKeyPropertyCache)
            {
                Func<string> func;
                if (GetSecurityKeyPropertyCache.TryGetValue(entityType, out func))
                    return func();
                func = (Func<string>)
                       Delegate.CreateDelegate(typeof(Func<string>), getSecurityKeyMethod.MakeGenericMethod(entityType));
                GetSecurityKeyPropertyCache[entityType] = func;
                return func();
            }
        }

        internal static string GetSecurityKeyPropertyInternal<TEntity>()
        {
            return AppCore.Instance.Resolve<IEntityInformationExtractor<TEntity>>().SecurityKeyPropertyName;
        }

        public static void Configure(Configuration cfg)
        {
            cfg.AddAssembly(typeof(User).Assembly);
            new UserMapper(cfg, typeof(User)).Map();
        }
    }
}
