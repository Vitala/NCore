using NCore.Domain;
using NCore.EntityFramework.Infrastructure;
using NCore.Kernel;
using System;
using System.Data.Entity;

namespace NCore.EntityFramework.Domain
{
    public class EfUnitOfWork : IUnitOfWorkImplementation
    {
        public IAppScope Scope { get; private set; }

        public IDbContext DbContext { get; private set; }
        
        public bool IsDisposed { get; private set; }

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public EfUnitOfWork(IAppScope scope, ICurrentUnitOfWorkProvider uowProvider)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            if (uowProvider == null)
                throw new ArgumentNullException("uowProvider");
            
            _currentUnitOfWorkProvider = uowProvider;

            if (_currentUnitOfWorkProvider.Current != null)
                throw new NCoreException("В текущем контексте уже открыт юнит-оф-ворк. Закройте его перед тем как открывать новый.");

            _currentUnitOfWorkProvider.Current = this;

            Scope = scope.BeginScope();

            var dbContextFactory = Scope.Resolve<IDbContextFactory>();
            if (dbContextFactory == null)
                throw new NCoreException("Не удалось инициализировать UoW EF, не удалос получить фабрику контекста подключения");

            DbContext = dbContextFactory.CreateDbContext();
        }

        public IRepositoryFactory RepositoryFactory
        {
            get { return Scope.Resolve<IRepositoryFactory>(); }
        }

        public ITransaction BeginTransaction(TransactionCloseType closeType = TransactionCloseType.Auto)
        {
            return new EfTransaction(DbContext.BeginTransaction(), closeType);
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                _currentUnitOfWorkProvider.Current = null;

                this.DbContext.Dispose();

                Scope.Dispose();

                IsDisposed = true;
            }
        }
    }
}
