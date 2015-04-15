using FluentNHibernate.Mapping;

namespace NCore.NHibernate.Security.Model
{
    public class EntityType : NamedEntity
    {
    }

    public class EntityTypeMap : ClassMap<EntityType>
    {
        public EntityTypeMap()
        {
            Table("EntityTypes");
            Id(x => x.Id);
            Map(x => x.Name);
        }
    }
}
