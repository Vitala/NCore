using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace NCore.NHibernate.Security.Model
{
    public class EntitiesGroup : NamedEntity
    {
        public EntitiesGroup()
        {
            Entities = new HashSet<EntityReference>();
            AllParents = new HashSet<EntitiesGroup>();
            AllChildren = new HashSet<EntitiesGroup>();
            DirectChildren = new HashSet<EntitiesGroup>();
        }

        public virtual ICollection<EntityReference> Entities { get; set; }
        public virtual EntitiesGroup Parent { get; set; }
        public virtual ICollection<EntitiesGroup> DirectChildren { get; set; }
        public virtual ICollection<EntitiesGroup> AllChildren { get; set; }
        public virtual ICollection<EntitiesGroup> AllParents { get; set; }
    }


    public class EntitiesGroupMap : ClassMap<EntitiesGroup>
    {
        public EntitiesGroupMap()
        {
            Table("EntitiesGroups");
            Id(x => x.Id);
            Map(x => x.Name);
            References(x => x.Parent).Column("Parent");

            HasManyToMany(x => x.AllChildren)
                .AsSet()
                .Table("EntityGroupsHierarchy")
                .LazyLoad()
                .Inverse()
                .ParentKeyColumn("ParentGroup")
                .ChildKeyColumn("ChildGroup")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasManyToMany(x => x.AllParents)
                .AsSet()
                .Table("EntityGroupsHierarchy")
                .LazyLoad()
                .ParentKeyColumn("ChildGroup")
                .ChildKeyColumn("ParentGroup")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasMany(x => x.DirectChildren)
                .AsSet()
                .Table("EntitiesGroups")
                .LazyLoad()
                .Inverse()
                .KeyColumn("Parent")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasManyToMany(x => x.Entities)
                .AsSet()
                .Table("EntityReferencesToEntitiesGroups")
                .LazyLoad()
                .ParentKeyColumn("GroupId")
                .ChildKeyColumn("EntityReferenceId")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");
        }
    }
}
