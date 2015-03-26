using NCore.Domain;
using NCore.Kernel;
using NHibernate;
using NHibernate.Linq;
using System.Linq;

namespace NCore.NHibernate.Domain
{
    public class NhRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        protected readonly ISession _session;

        public NhRepository(ICurrentSessionProvider currentSessionProvider)
        {
            _session = currentSessionProvider.CurrentSession;
            if (_session == null)
                throw new NCoreException("Невозможно использовать репозиторий без контекста юнит-оф-ворк. Откройте новый юнит-оф-ворк перед созданием репозитория.");
        }

        public IQueryable<TEntity> GetAll()
        {
            return _session.Query<TEntity>();
        }

        public TEntity Get(TPrimaryKey key)
        {
            return _session.Get<TEntity>(key);
        }

        public void Insert(TEntity entity)
        {
            _session.Save(entity);
        }

        public void Update(TEntity entity)
        {
            _session.Update(entity);
        }

        public void Delete(TPrimaryKey id)
        {
            _session.Delete(_session.Load<TEntity>(id));
        }
    }
}