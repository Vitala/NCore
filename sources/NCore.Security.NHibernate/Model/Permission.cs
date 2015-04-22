using FluentNHibernate.Mapping;
using NCore.Domain;
using NCore.Security.Model;
using System;

namespace NCore.Security.NHibernate.Model
{
    public class Permission : Entity<int>
    {
        public virtual Operation Operation { get; set; }
        public virtual Guid? EntitySecurityKey { get; set; }
        public virtual bool Allow { get; set; }
        public virtual User User { get; set; }
        public virtual UsersGroup UsersGroup { get; set; }
        public virtual EntitiesGroup EntitiesGroup { get; set; }
        public virtual int Level { get; set; }
        public virtual string EntityTypeName { get; set; }

        public virtual void SetEntityType(Type type)
        {
            EntityTypeName = String.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }
    }

    public class PermissionMap : ClassMap<Permission>
    {
        public PermissionMap()
        {
            Table("Permissions");
            Id(x => x.Id);
            Map(x => x.EntitySecurityKey).Nullable();
            Map(x => x.Allow);
            Map(x => x.Level);
            Map(x => x.EntityTypeName);

            References(x => x.Operation).Column("Operation");
            References(x => x.User).Column("User");
            References(x => x.UsersGroup).Column("UsersGroup");
            References(x => x.EntitiesGroup).Column("EntitiesGroup");
        }
    }
}
