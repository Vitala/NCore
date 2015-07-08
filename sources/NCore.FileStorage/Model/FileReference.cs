using NCore.Domain;
using System;
using System.Collections.Generic;

namespace NCore.FileStorage.Model
{
    public class FileReference : Entity<int>
    {
        public virtual string Directory { get; set; }
        public virtual string RelativePath { get; set; }
        public virtual DateTime DateUploaded { get; set; }
        public virtual FileStorageModes Mode { get; set; }
        public virtual Dictionary<string, string> Metadata { get; set; }
        //public virtual Guid MultiFileEntityKey { get; set; }
       
    }
}
