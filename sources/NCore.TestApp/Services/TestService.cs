using NCore.TestApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCore.TestApp.Services
{
    public class TestService : ITestService
    {
        private readonly Func<IUnitOfWork> _uowFactory;

        public TestService(Func<IUnitOfWork> uowFactory)
        {
            _uowFactory = uowFactory;
        }

        public void AddTestRecord(string name)
        {
            using (var uow = _uowFactory())
            {
                var testRepo = uow.Repository<TestEntity, int>();
                uow.BeginTransaction();
                testRepo.Insert(new TestEntity() { Name = name });
                uow.Commit();
            }
        }
    }
}
