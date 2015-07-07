using NCore.FileStorage.Model;
using System;

namespace NCore.FileStorage.Configuration
{
    public interface IFileStorageConfiguration
    {
        FileStorageModes Mode { get; }
        String Directory { get; }
        //TODO: ftp params
    }
}
