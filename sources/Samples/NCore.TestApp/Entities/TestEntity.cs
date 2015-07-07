using FluentNHibernate.Mapping;
using NCore.Domain;
using NCore.FileStorage.Model;
using NCore.Security.Model;
using System;

namespace NCore.TestApp.Entities
{
    public class TestEntityMap : ClassMap<TestEntity>
    {
        public TestEntityMap()
        {
            Id(x => x.Id).GeneratedBy.Sequence("test_id_seq");
            Map(x => x.Name);
            Map(x => x.SecurityKey);
            Table("Test");

            References(x => x.File);
        }
    }
    
    public class TestEntity : SingleFileEntity<int>, IEntityInformationExtractor<TestEntity>
    {
        public virtual string Name { get; set; }
        public virtual Guid SecurityKey { get; set; }

        public override FileReference File { get; set; }
    }
}
