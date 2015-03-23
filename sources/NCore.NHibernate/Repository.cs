using NHibernate;
using NHibernate.Linq;
using System.Linq;

namespace NCore.NHibernate
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        protected readonly ISession _session;

        public Repository(INhUnitOfWork uow)
        {
            _session = uow.Session;
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
