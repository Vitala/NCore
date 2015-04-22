using NCore.NHibernate.Domain;
using NCore.Security.NHibernate.Interfaces;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCore.NHibernate;

namespace NCore.TestApp.Entities
{
    public class TestRepository : NhRepository<TestEntity, int>, ITestRepository
    {
        public TestRepository(ICurrentSessionProvider currentSessionProvider)
            : base(currentSessionProvider)
        {
            
        }

        public ICriteria GetEntities()
        {
            var criteria = _session.CreateCriteria<TestEntity>();
            return criteria;
        }
    }
}
