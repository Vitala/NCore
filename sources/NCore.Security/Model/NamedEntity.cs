using NCore.Domain;

namespace NCore.Security.Model
{
    //TODO: реализовать кастомного поставщика методов equals и gethashcode
    public class NamedEntity : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
