using Autofac;
using NHibernate;

namespace NCore.NHibernate
{
    public class UnitOfWork : INhUnitOfWork, IRepositoryFactory
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IAppScope _scope;
        private ITransaction _transaction;
        private ISession _session;

        public ISession Session { get { return _session; } }

        public UnitOfWork(ISessionFactory sessionFactory, IAppScope scope)
        {
            _sessionFactory = sessionFactory;

            var cb = new ContainerBuilder();
            cb.RegisterInstance(this).As<IUnitOfWork>().As<INhUnitOfWork>().AsSelf().SingleInstance();
            scope.Update(cb);
            _scope = scope;

            _session = _sessionFactory.OpenSession();
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
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
            if (_session != null)
            {
                _session.Dispose();
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
