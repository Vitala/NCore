using NCore.FileStorage.Model;
using System;
using System.Configuration;

namespace NCore.FileStorage.Configuration
{
    public class FileStorageConfiguration : ConfigurationSection, IFileStorageConfiguration
    {
        public const String SectionName = "FileStorageConfiguration";

        public static FileStorageConfiguration DefaultConfiguration { get { return new FileStorageConfiguration(); } }

        public static FileStorageConfiguration Load()
        {
            return ConfigurationManager.GetSection(SectionName) as FileStorageConfiguration;
        }

        [ConfigurationProperty("Mode", IsRequired = true)]
        public FileStorageModes Mode
        {
            get
            {
                return (FileStorageModes)this["Mode"];
            }
        }

        [ConfigurationProperty("Directory", IsRequired = false)]
        public String Directory { get { return this["Directory"] as String; } }
    }
}
