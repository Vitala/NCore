using FluentNHibernate.Mapping;
using NCore.Domain;
using System;

namespace NCore.Security.NHibernate.Model
{
    public class EntityReference : Entity<int>
    {
        public virtual Guid EntitySecurityKey { get; set; }
        public virtual EntityType Type { get; set; }
    }

    public class EntityReferenceMap : ClassMap<EntityReference>
    {
        public EntityReferenceMap()
        {
            Table("EntityReferences");
            Id(x => x.Id);
            Map(x => x.EntitySecurityKey);
            References(x => x.Type).Column("Type");
        }

    }
}
