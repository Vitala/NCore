
namespace NCore
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity, TPrimaryKey> Repository<TEntity, TPrimaryKey>() where TEntity : Entity<TPrimaryKey>;
    }
}
