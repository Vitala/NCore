using System;

namespace NCore.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        bool IsDisposed { get; }
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
