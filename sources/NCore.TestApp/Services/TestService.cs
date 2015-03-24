using NCore.TestApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void AddTestRecord()
        {
            var t1 = new Task(() =>
            {
                using (var uow = _uowFactory())
                {
                    var testRepo = uow.Repository<TestEntity, int>();
                    uow.BeginTransaction();
                    testRepo.Insert(new TestEntity() { Name = "task 1" });
                    Thread.Sleep(5000);
                    uow.Commit();
                }
            });
            var t2 = new Task(() =>
            {
                using (var uow = _uowFactory())
                {
                    var testRepo = uow.Repository<TestEntity, int>();
                    uow.BeginTransaction();
                    testRepo.Insert(new TestEntity() { Name = "task 2" });
                    Thread.Sleep(10000);
                    uow.Commit();
                }
            });
            t1.Start(); t2.Start();
        }
    }
}
