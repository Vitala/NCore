using NCore.Domain;

namespace NCore.NHibernate.Security.Model
{
    //TODO: реализовать кастомного поставщика методов equals и gethashcode
    public class NamedEntity : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
