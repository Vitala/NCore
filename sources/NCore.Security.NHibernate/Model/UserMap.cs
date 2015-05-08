using FluentNHibernate.Mapping;
using NCore.Security.Model;

namespace NCore.Security.NHibernate.Model
{
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
