
namespace NCore.NHibernate.Security
{
    public class SecurityInfo
    {
        public SecurityInfo(string name, object identifier)
        {
            Name = name;
            Identifier = identifier;
        }

        public virtual object Identifier { get; private set; }
        public virtual string Name { get; private set; }
    }
}
