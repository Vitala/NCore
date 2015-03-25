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
        private readonly Func<IRepository<TestEntity, int>> _repoFactory;

        public TestService(Func<IUnitOfWork> uowFactory, Func<IRepository<TestEntity, int>> repoFactory)
        {
            _uowFactory = uowFactory;
            _repoFactory = repoFactory;

        }

        public void AddTestRecord()
        {
            var t1 = new Task(() =>
            {
                using (var uow = _uowFactory())
                {
                    var testRepo = _repoFactory();
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
                    var testRepo = _repoFactory();
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
