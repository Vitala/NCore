using FluentNHibernate.Cfg;
using NCore.FileStorage.NHibernate.Postgre.Model;

namespace NCore.FileStorage.NHibernate.Postgre
{
    /// <summary>
    /// Ядро системы хранения файлов
    /// </summary>
    public static class FileStorageCore
    {
        public static void Configure(FluentConfiguration cfg)
        {
            cfg.Mappings(x => x.FluentMappings.AddFromAssemblyOf<FileReferenceMap>());
        }
    }
}
