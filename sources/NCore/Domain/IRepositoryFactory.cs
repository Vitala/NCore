namespace NCore.Domain
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>;
        TRepository Repository<TRepository>() where TRepository : IRepository;
    }
}
