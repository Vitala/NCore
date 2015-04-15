using FluentNHibernate.Mapping;

namespace NCore.NHibernate.Security.Model
{
    public class User : NamedEntity
    {
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(x => x.Id);
            Map(x => x.Name);
        }
    }
}
