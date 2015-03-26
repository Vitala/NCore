using NCore.Domain;
using NHibernate;
using System;

namespace NCore.NHibernate.Domain
{
    public class NhUnitOfWork : IUnitOfWork
    {
        public ISession Session { get; private set; }
        public bool IsDisposed { get; private set; }

        private ITransaction _transaction;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWokProvider;

        public NhUnitOfWork(ICurrentUnitOfWorkProvider currentUnitOfWokProvider,
                          ISessionFactory sessionFactory)
        {
            _currentUnitOfWokProvider = currentUnitOfWokProvider;
            Session = sessionFactory.OpenSession();
            _currentUnitOfWokProvider.Current = this;
        }

        public void BeginTransaction()
        {
           _transaction = Session.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                _currentUnitOfWokProvider.Current = null;

                if (Session != null && Session.IsOpen)
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
                        _transaction.Dispose();
                        Session.Dispose();
                        IsDisposed = true;
                    }
                }
            }
        }

    }
}
