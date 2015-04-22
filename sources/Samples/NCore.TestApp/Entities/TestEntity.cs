﻿using FluentNHibernate.Mapping;
using NCore.Domain;
using NCore.Security.NHibernate.Interfaces;
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
            Map(x => x.SecurityKey);
            Table("Test");
        }
    }

    public class TestEntity : Entity<int>, IEntityInformationExtractor<TestEntity>
    {
        public virtual string Name { get; set; }
        public virtual Guid SecurityKey { get; set; }
    }
}
