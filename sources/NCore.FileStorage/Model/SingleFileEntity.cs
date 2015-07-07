using NCore.Domain;

namespace NCore.FileStorage.Model
{
    public abstract class SingleFileEntity<TPrimaryKey> : Entity<TPrimaryKey>
    {
        public abstract FileReference File { get; set; }
     
    }
}
