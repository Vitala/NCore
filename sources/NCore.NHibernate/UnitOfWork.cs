using Autofac;
using NHibernate;

namespace NCore.NHibernate
{
    public class UnitOfWork : INhUnitOfWork, IRepositoryFactory
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IAppScope _scope;
        private ITransaction _transaction;

        public ISession Session { get; private set; }

        public UnitOfWork(ISessionFactory sessionFactory, IAppScope scope)
        {
            _sessionFactory = sessionFactory;
            _scope = scope;
            Session = _sessionFactory.OpenSession();

            var cb = new ContainerBuilder();
            cb.RegisterInstance(Session).As<ISession>().SingleInstance();
            _scope.Update(cb);

        }

        public void BeginTransaction()
        {
            _transaction = Session.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
            }
            if (Session != null)
            {
                Session.Dispose();
            }
            if (_scope != null)
            {
                _scope.Dispose();
            }
        }

        public IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>
        {
            return _scope.Resolve<IRepository<TEntity, TPrimaryKey>>();
        }
    }
}
