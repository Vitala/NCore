using NCore.Domain;
using NCore.Kernel;
using NHibernate;
using System;

namespace NCore.NHibernate.Domain
{
    using ITransaction = NCore.Domain.ITransaction;

    public class NhUnitOfWork : IUnitOfWorkImplementation
    {
        public IAppScope Scope { get; private set; }

        public IRepositoryFactory RepositoryFactory { get { return Scope.Resolve<IRepositoryFactory>(); } }

        public ISession Session { get; private set; }
        public bool IsDisposed { get; private set; }

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public NhUnitOfWork(IAppScope scope, ICurrentUnitOfWorkProvider uowProvider)
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

            var sessionFactory = Scope.Resolve<ISessionFactory>();
            if (sessionFactory == null)
                throw new NCoreException("Не удалось инициализировать UoW Nh, не удалос получить фабрику сессий");

            Session = sessionFactory.OpenSession();
        }

        public ITransaction BeginTransaction(TransactionCloseType closeType = TransactionCloseType.Auto)
        {
            return new NhTransaction(Session.BeginTransaction(), closeType);
        }

        public void SaveChanges()
        {
            Session.Flush();
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                _currentUnitOfWorkProvider.Current = null;

                if (Session != null && Session.IsOpen)                
                    Session.Dispose();

                Scope.Dispose();
                
                IsDisposed = true;
            }
        }
    }
}
