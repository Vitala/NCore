using NCore.NHibernate.Security.Model;
using NHibernate;
using NHibernate.Criterion;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IAuthorizationService
    {
        void AddPermissionsToQuery(User user, string operation, ICriteria criteria);
        void AddPermissionsToQuery(UsersGroup usersgroup, string operation, ICriteria criteria);
        void AddPermissionsToQuery(User user, string operation, DetachedCriteria criteria);
        void AddPermissionsToQuery(UsersGroup usersgroup, string operation, DetachedCriteria criteria);

        bool IsAllowed<TEntity>(User user, TEntity entity, string operation) where TEntity : class;
        bool IsAllowed(User user, string operation);

        AuthorizationInformation GetAuthorizationInformation(User user, string operation);
        AuthorizationInformation GetAuthorizationInformation<TEntity>(User user, TEntity entity, string operation) where TEntity : class;
    }
}
