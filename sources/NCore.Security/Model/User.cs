using NCore.Domain;
namespace NCore.Security.Model
{
    public class User : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
