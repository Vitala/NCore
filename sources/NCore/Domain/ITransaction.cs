using System;

namespace NCore.Domain
{
    public interface ITransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}
