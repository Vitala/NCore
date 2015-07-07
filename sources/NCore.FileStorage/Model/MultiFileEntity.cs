using NCore.Domain;
using NCore.FileStorage.Model;
using System;
using System.Collections.Generic;

namespace NCore.FileStorage.Model
{
    //TODO: придумать как безболезненно реализовать связь
    abstract class IMultiFileEntity
    {
        public abstract Guid MultiFileEntityKey { get; set; }
        //public abstract ICollection<FileReference> Files { get; set; }
    }
}
