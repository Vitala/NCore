using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace NCore.NHibernate.Security.Model
{
    public class Operation : NamedEntity
    {
        public Operation()
        {
            Children = new HashSet<Operation>();
        }

        public virtual string Comment { get; set; }
        public virtual Operation Parent { get; set; }
        public virtual ICollection<Operation> Children { get; set; }
    }

    public class OperationMap : ClassMap<Operation>
    {
        public OperationMap()
        {
            Table("Operations");
            Id(x => x.Id);
            Map(x => x.Comment);
            Map(x => x.Name);
            References(x => x.Parent).Column("Parent");

            HasMany(x => x.Children)
                .AsSet()
                .Table("Operations")
                .LazyLoad()
                .Inverse()
                .KeyColumn("Parent")
                .Cache
                .ReadWrite()
                .Region("NCoreSecurity");
        }
    }
}
