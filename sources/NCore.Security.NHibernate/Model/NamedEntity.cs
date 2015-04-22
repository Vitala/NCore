using NCore.Domain;

namespace NCore.Security.NHibernate.Model
{
    //TODO: реализовать кастомного поставщика методов equals и gethashcode
    public class NamedEntity : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
