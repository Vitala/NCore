using NHibernate;
using System;

namespace NCore.NHibernate
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction;
        private readonly ICurrentSessionProvider _currentSessionProvider;

        public UnitOfWork(ICurrentSessionProvider currentSessionProvider)
        {
            _currentSessionProvider = currentSessionProvider;
            _session = currentSessionProvider.OpenSession();
#if DEBUG
            _session.FlushMode = FlushMode.Commit;
#endif
        }

        public void BeginTransaction()
        {
           _transaction =_session.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                _currentSessionProvider.Dispose();

                if (_session != null && _session.IsOpen)
                {
                    try
                    {
                        if (_transaction != null && _transaction.IsActive)
                        {
                            _transaction.Rollback();
                        }
                    }
                    finally
                    {
                        _session.Dispose();
                        disposed = true;
                    }
                }
            }
        }
    }
}
