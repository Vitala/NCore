using FluentNHibernate.Mapping;
using NCore.FileStorage.Model;
using System.Collections.Generic;

namespace NCore.FileStorage.NHibernate.Postgre.Model
{
    public class FileReferenceMap : ClassMap<FileReference>
    {
        public FileReferenceMap()
        {
            Table("FileReferences");
            Id(x => x.Id);
            Map(x => x.FilePath);
            Map(x => x.DateUploaded);
            Map(x => x.Mode).CustomType<FileStorageModes>();
            Map(x => x.Metadata).CustomType<Blobbed<Dictionary<string, string>>>();
            //Map(x => x.MultiFileEntityKey);
        }
    }
}
