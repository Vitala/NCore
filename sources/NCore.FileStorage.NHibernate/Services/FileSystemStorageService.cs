using NCore.FileStorage.Configuration;
using NCore.FileStorage.Model;
using NCore.FileStorage.NHibernate.Interfaces;
using NCore.Kernel;
using NCore.NHibernate;
using NHibernate;
using System;
using System.Collections.Generic;
using System.IO;

namespace NCore.FileStorage.NHibernate.Services
{
    public class FileSystemStorageService<TPrimaryKey> : IStorageService<TPrimaryKey>
    {
        private readonly ISession _session;
        private readonly IFileStorageConfiguration _config;

        public FileSystemStorageService(ICurrentSessionProvider currentSessionProvider,
                                        IFileStorageConfiguration config)
        {
            _session = currentSessionProvider.CurrentSession;
            _config = config;
            if (_session == null)
                throw new NCoreException("Невозможно использовать сервис без контекста юнит-оф-ворк. Откройте новый юнит-оф-ворк перед созданием сервиса.");
        }

        private void CheckDirectoryValidity()
        {
            if (String.IsNullOrWhiteSpace(_config.Directory))
                throw new NCoreFileStorageException("Директория сохранения не задана в конфигурации");

            if (!Directory.Exists(_config.Directory))
                Directory.CreateDirectory(_config.Directory);
        }

        private void CheckAndRemoveOldFiles(SingleFileEntity<TPrimaryKey> entity)
        {
            if (entity.File != null)
            {
                File.Delete(entity.File.FilePath);
                var dir = Path.GetDirectoryName(entity.File.FilePath);
                Directory.Delete(dir);
                entity.File = null;
            }
        }

        public void AttachFile(SingleFileEntity<TPrimaryKey> entity, string fileName, byte[] content, Dictionary<string, string> metadata)
        {
            CheckDirectoryValidity();
            CheckAndRemoveOldFiles(entity);

            var subdir = Path.Combine(_config.Directory, Guid.NewGuid().ToString());
            if (!Directory.Exists(subdir))
                Directory.CreateDirectory(subdir);
            var path = Path.Combine(subdir, fileName);
            File.WriteAllBytes(path, content);

            var fileData = new FileReference()
            {
                DateUploaded = DateTime.Now,
                Metadata = metadata,
                Mode = FileStorageModes.FileSystem,
                FilePath = path
            };
            _session.Save(fileData);
            entity.File = fileData;
        }

        public IEnumerable<string> GetAttachedFilesList(SingleFileEntity<TPrimaryKey> entity)
        {
            throw new NotImplementedException();
        }

       public byte[] GetFileContent(SingleFileEntity<TPrimaryKey> entity, string fileName)
        {
            throw new NotImplementedException();
        }

       public void DeleteFile(SingleFileEntity<TPrimaryKey> entity, string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
