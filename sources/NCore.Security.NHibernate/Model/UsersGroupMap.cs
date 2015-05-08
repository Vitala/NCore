using FluentNHibernate.Mapping;
using NCore.Security.Model;
using System.Collections.Generic;

namespace NCore.Security.NHibernate.Model
{
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
