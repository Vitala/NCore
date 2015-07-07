using Autofac;
using NCore.FileStorage.Configuration;
using NCore.FileStorage.Model;
using NCore.FileStorage.NHibernate.Interfaces;
using NCore.FileStorage.NHibernate.Services;

namespace NCore.FileStorage.NHibernate
{
    public class NCoreFileStorageModule  : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var cnf = FileStorageConfiguration.Load();

            builder.RegisterInstance(cnf)
                            .As<IFileStorageConfiguration>()
                            .AsSelf()
                            .SingleInstance();

            //В зависимости от режима хранения регистрируем разные сервисы
            if (cnf.Mode == FileStorageModes.FileSystem)
            {
                builder.RegisterGeneric(typeof(FileSystemStorageService<>)).As(typeof(IStorageService<>));
            }
        }
    }
}
