using NCore.Domain;
using NCore.NHibernate.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.TestApp.Entities
{
    public interface ITestRepository : IRepository<TestEntity, int>
    {
    ICriteria GetEntities();
    }
}
