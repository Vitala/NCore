using System;

namespace NCore
{
    public interface IUnitOfWork : IRepositoryFactory, IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
