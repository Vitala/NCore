using NCore.Domain;
using NCore.Kernel;
using NHibernate;
using System;

namespace NCore.NHibernate.Domain
{
    public class NhUnitOfWork : IUnitOfWork
    {
        public ISession Session { get; private set; }
        public bool IsDisposed { get; private set; }

        private ITransaction _transaction;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public NhUnitOfWork(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
                          ISessionFactory sessionFactory)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            if (_currentUnitOfWorkProvider.Current != null)
                throw new NCoreException("В текущем контексте уже открыт юнит-оф-ворк. Закройте его перед тем как открывать новый.");
            Session = sessionFactory.OpenSession();
            _currentUnitOfWorkProvider.Current = this;
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
                _currentUnitOfWorkProvider.Current = null;

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
                        if (_transaction != null)
                        {
                            _transaction.Dispose();
                        }
                        Session.Dispose();
                        IsDisposed = true;
                    }
                }
            }
        }

    }
}
