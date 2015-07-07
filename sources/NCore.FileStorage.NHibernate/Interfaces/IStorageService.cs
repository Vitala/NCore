using NCore.Domain;
using NCore.FileStorage.Model;
using System.Collections.Generic;

namespace NCore.FileStorage.NHibernate.Interfaces
{
    public interface IStorageService<TPrimaryKey>
    {
        void AttachFile(SingleFileEntity<TPrimaryKey> entity, string fileName, byte[] content, Dictionary<string, string> metadata);
        IEnumerable<string> GetAttachedFilesList(SingleFileEntity<TPrimaryKey> entity);
        byte[] GetFileContent(SingleFileEntity<TPrimaryKey> entity, string fileName);
        void DeleteFile(SingleFileEntity<TPrimaryKey> entity, string fileName);
    }
}
