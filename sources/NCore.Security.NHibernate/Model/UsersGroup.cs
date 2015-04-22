using FluentNHibernate.Mapping;
using NCore.Security.Model;
using System.Collections.Generic;

namespace NCore.Security.NHibernate.Model
{
    public class UsersGroup : NamedEntity
    {
	    public UsersGroup()
	    {
	        Users = new HashSet<User>();
	        AllParents = new HashSet<UsersGroup>();
	        AllChildren = new HashSet<UsersGroup>();
	        DirectChildren = new HashSet<UsersGroup>();
	    }

	    public virtual ICollection<User> Users { get; set; }
	    public virtual UsersGroup Parent { get; set; }
	    public virtual ICollection<UsersGroup> DirectChildren { get; set; }
	    public virtual ICollection<UsersGroup> AllChildren { get; set; }
	    public virtual ICollection<UsersGroup> AllParents { get; set; }
    }

    public class UsersGroupMap : ClassMap<UsersGroup>
    {
        public UsersGroupMap()
        {
            Table("UsersGroups");
            Id(x => x.Id);
            Map(x => x.Name).Unique();

            References(x => x.Parent).Column("Parent");

            HasManyToMany(x => x.AllChildren)
                .AsSet()
                .Table("UsersGroupsHierarchy")
                .LazyLoad()
                .Inverse()
                .ParentKeyColumn("ParentGroup")
                .ChildKeyColumn("ChildGroup")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasManyToMany(x => x.AllParents)
                .AsSet()
                .Table("UsersGroupsHierarchy")
                .LazyLoad()
                .ParentKeyColumn("ChildGroup")
                .ChildKeyColumn("ParentGroup")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasMany(x => x.DirectChildren)
                .AsSet()
                .Table("UsersGroups")
                .LazyLoad()
                .Inverse()
                .KeyColumn("Parent")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");

            HasManyToMany(x => x.Users)
                .AsSet()
                .Table("UsersToUsersGroups")
                .LazyLoad()
                .ParentKeyColumn("GroupId")
                .ChildKeyColumn("UserId")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity"); 
        }
    }
}
