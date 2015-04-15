﻿using NCore.Domain;
using NCore.EntityFramework.Infrastructure;
using NCore.Kernel;
using System.Data.Entity;

namespace NCore.EntityFramework.Domain
{
    public class EfUnitOfWork : IUnitOfWork
    {
        public IDbContext DbContext { get; private set; }
        public bool IsDisposed { get; private set; }

        private DbContextTransaction _transaction;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public EfUnitOfWork(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IDbContextFactory dbContextFactory)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            DbContext = dbContextFactory.CreateDbContext();
            if (_currentUnitOfWorkProvider.Current != null)
                throw new NCoreException("В текущем контексте уже открыт юнит-оф-ворк. Закройте его перед тем как открывать новый.");
            _currentUnitOfWorkProvider.Current = this;
        }


        public void BeginTransaction()
        {
            _transaction = this.DbContext.BeginTransaction();
        }

        public void Commit()
        {
            this.DbContext.SaveChanges();
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

                if (_transaction != null)
                {
                    try
                    {
                        if (_transaction.UnderlyingTransaction.Connection != null)
                            _transaction.Rollback();
                    }
                    finally
                    {
                        _transaction.Dispose();
                    }
                }

                this.DbContext.Dispose();
                IsDisposed = true;
            }
        }
    }
}
