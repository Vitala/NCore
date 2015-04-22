using Autofac;

namespace NCore.Domain
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RepositoryFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>
        {
            return _lifetimeScope.Resolve<IRepository<TEntity, TPrimaryKey>>();
        }

        public TRepository Repository<TRepository>() where TRepository : IRepository
        {
            return _lifetimeScope.Resolve<TRepository>();
        }
    }
}
