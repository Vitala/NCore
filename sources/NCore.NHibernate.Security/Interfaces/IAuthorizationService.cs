using NCore.NHibernate.Security.Model;
using NHibernate;
using NHibernate.Criterion;
using System.Linq;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IAuthorizationService
    {
        IQueryable<TEntity> AddPermissionsToQuery<TEntity>(User user, string operation, IQueryable<TEntity> query)
            where TEntity : IEntityInformationExtractor<TEntity>;
        void AddPermissionsToQuery<TEntity>(UsersGroup usersGroup, string operation, IQueryable<TEntity> query)
            where TEntity : IEntityInformationExtractor<TEntity>;

        void AddPermissionsToQuery(User user, string operation, ICriteria criteria);
        void AddPermissionsToQuery(UsersGroup usersgroup, string operation, ICriteria criteria);

        void AddPermissionsToQuery(User user, string operation, DetachedCriteria criteria);
        void AddPermissionsToQuery(UsersGroup usersgroup, string operation, DetachedCriteria criteria);

        bool IsAllowed<TEntity>(User user, TEntity entity, string operation) where TEntity : IEntityInformationExtractor<TEntity>;
        bool IsAllowed(User user, string operation);
    }
}
