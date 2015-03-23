using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
