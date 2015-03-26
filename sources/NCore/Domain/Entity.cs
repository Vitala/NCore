
namespace NCore.Domain
{
    public class Entity<TPrimaryKey>: IEntity<TPrimaryKey>
    {
        public virtual TPrimaryKey Id { get; set; }
    }
}
