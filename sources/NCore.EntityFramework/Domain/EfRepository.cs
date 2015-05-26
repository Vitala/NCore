using NCore.Domain;
using NCore.EntityFramework.Infrastructure;
using NCore.Kernel;
using System.Data.Entity;
using System.Linq;

namespace NCore.EntityFramework.Domain
{
    public class EfRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        private readonly IDbContext _context;
        protected readonly DbSet<TEntity> _set;

        public EfRepository(ICurrentDbContextProvider currentDbContextProvider)
        {
            _context = currentDbContextProvider.CurrentContext;
            if (_context == null)
                throw new NCoreException("Невозможно использовать репозиторий без контекста юнит-оф-ворк. Откройте новый юнит-оф-ворк перед созданием репозитория.");
            //TODO: make each method of repository transactional if _context == null. In that case repository can  be resolved without unit of work
            _set = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _set;
        }

        public TEntity Get(TPrimaryKey key)
        {
            return _set.Find(key);
        }

        public void Insert(TEntity entity)
        {
            _set.Add(entity);
        }

        public void Update(TEntity entity)
        {
            AttachIfNot(entity);

            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TPrimaryKey id)
        {
            _set.Remove(_set.Find(id));
        }

        public void Delete(TEntity entity)
        {
            _set.Remove(entity);
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!_set.Local.Contains(entity))
            {
                _set.Attach(entity);
            }
        }
    }
}
