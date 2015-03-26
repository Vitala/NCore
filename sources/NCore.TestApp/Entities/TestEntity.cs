using FluentNHibernate.Mapping;
using NCore.Domain;

namespace NCore.TestApp.Entities
{
    public class TestEntityMap : ClassMap<TestEntity>
    {
        public TestEntityMap()
        {
            Id(x => x.Id).GeneratedBy.Sequence("test_id_seq");
            Map(x => x.Name);
            Table("Test");
        }
    }

    public class TestEntity : Entity<int>
    {
        public virtual string Name { get; set; }
    }
}
