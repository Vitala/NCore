using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using System;

namespace NCore.NHibernate.Security.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthorizationRepository _authorizationRepository;

        public AuthorizationService(IAuthorizationRepository authorizationRepository)
        {
            _authorizationRepository = authorizationRepository;
        }

        #region IAuthorizationService Members

        public void AddPermissionsToQuery(User user, string operation, ICriteria criteria)
        {
            var allowed = GetPermissionQueryInternal(user, operation, GetSecurityKeyProperty(criteria));
            criteria.Add(allowed);
        }

        public void AddPermissionsToQuery(UsersGroup usersgroup, string operation, ICriteria criteria)
        {
            var allowed = GetPermissionQueryInternal(usersgroup, operation, GetSecurityKeyProperty(criteria));
            criteria.Add(allowed);
        }

        public void AddPermissionsToQuery(User user, string operation, DetachedCriteria criteria)
        {
            var allowed = GetPermissionQueryInternal(user, operation, GetSecurityKeyProperty(criteria));
            criteria.Add(allowed);
        }

        public void AddPermissionsToQuery(UsersGroup usersgroup, string operation, DetachedCriteria criteria)
        {
            var allowed = GetPermissionQueryInternal(usersgroup, operation, GetSecurityKeyProperty(criteria));
            criteria.Add(allowed);
        }

        public bool IsAllowed<TEntity>(User user, TEntity entity, string operation) where TEntity : class
        {
            var permissions = _authorizationRepository.GetPermissionsFor(user, entity, operation);
            if (permissions.Length == 0)
                return false;
            return permissions[0].Allow;
        }

        public bool IsAllowed(User user, string operation)
        {
            var permissions = _authorizationRepository.GetGlobalPermissionsFor(user, operation);
            if (permissions.Length == 0)
                return false;
            return permissions[0].Allow;
        }

        public AuthorizationInformation GetAuthorizationInformation(User user, string operation)
        {
            AuthorizationInformation info;
            if (InitializeAuthorizationInfo(operation, out info))
                return info;
            var permissions = _authorizationRepository.GetGlobalPermissionsFor(user, operation);
            AddPermissionDescriptionToAuthorizationInformation<object>(operation, info, user, permissions, null);
            return info;
        }

        public AuthorizationInformation GetAuthorizationInformation<TEntity>(User user, TEntity entity,
                                                                             string operation) where TEntity : class
        {
            AuthorizationInformation info;
            if (InitializeAuthorizationInfo(operation, out info))
                return info;
            var permissions = _authorizationRepository.GetPermissionsFor(user, entity, operation);
            AddPermissionDescriptionToAuthorizationInformation(operation, info, user, permissions, entity);
            return info;
        }

        #endregion

        private static ICriterion GetPermissionQueryInternal(User user, string operation, string securityKeyProperty)
        {
            var operationNames = Strings.GetHierarchicalOperationNames(operation);
            var criteria = DetachedCriteria.For<Permission>("permission")
                .CreateAlias("Operation", "op")
                .CreateAlias("EntitiesGroup", "entityGroup", JoinType.LeftOuterJoin)
                .CreateAlias("entityGroup.Entities", "entityKey", JoinType.LeftOuterJoin)
                .SetProjection(Projections.Property("Allow"))
                .Add(Restrictions.In("op.Name", operationNames))
                .Add(Restrictions.Eq("User", user)
                || Subqueries.PropertyIn("UsersGroup.Id",
                                         SecurityCriterions.AllGroups(user).SetProjection(Projections.Id())))
                .Add(
                Property.ForName(securityKeyProperty).EqProperty("permission.EntitySecurityKey") ||
                Property.ForName(securityKeyProperty).EqProperty("entityKey.EntitySecurityKey") ||
                (
                    Restrictions.IsNull("permission.EntitySecurityKey") &&
                    Restrictions.IsNull("permission.EntitiesGroup")
                )
                )
                .SetMaxResults(1)
                .AddOrder(Order.Desc("Level"))
                .AddOrder(Order.Asc("Allow"));
            return Subqueries.Eq(true, criteria);
        }

        private ICriterion GetPermissionQueryInternal(UsersGroup usersgroup, string operation, string securityKeyProperty)
        {
            var operationNames = Strings.GetHierarchicalOperationNames(operation);
            var criteria = DetachedCriteria.For<Permission>("permission")
                .CreateAlias("Operation", "op")
                .CreateAlias("EntitiesGroup", "entityGroup", JoinType.LeftOuterJoin)
                .CreateAlias("entityGroup.Entities", "entityKey", JoinType.LeftOuterJoin)
                .SetProjection(Projections.Property("Allow"))
                .Add(Expression.In("op.Name", operationNames))
                .Add(Expression.Eq("UsersGroup", usersgroup))
                .Add(
                Property.ForName(securityKeyProperty).EqProperty("permission.EntitySecurityKey") ||
                Property.ForName(securityKeyProperty).EqProperty("entityKey.EntitySecurityKey") ||
                (
                    Expression.IsNull("permission.EntitySecurityKey") &&
                    Expression.IsNull("permission.EntitiesGroup")
                )
                )
                .SetMaxResults(1)
                .AddOrder(Order.Desc("Level"))
                .AddOrder(Order.Asc("Allow"));
            return Subqueries.Eq(true, criteria);
        }

        private string GetSecurityKeyProperty(DetachedCriteria criteria)
        {
            var rootType = criteria.GetRootEntityTypeIfAvailable();
            return criteria.Alias + "." + SecurityCore.GetSecurityKeyProperty(rootType);
        }

        private string GetSecurityKeyProperty(ICriteria criteria)
        {
            var rootType = criteria.GetRootEntityTypeIfAvailable();
            return criteria.Alias + "." + SecurityCore.GetSecurityKeyProperty(rootType);
        }


        private void AddPermissionDescriptionToAuthorizationInformation<TEntity>(string operation,
                                                                                 AuthorizationInformation info,
                                                                                 User user, Permission[] permissions,
                                                                                 TEntity entity)
            where TEntity : class
        {
            var entityDescription = "";
            var entitiesGroupsDescription = "";
            if (entity != null)
            {
               var entitiesGroups = _authorizationRepository.GetAssociatedEntitiesGroupsFor(entity);
                entityDescription = SecurityCore.Instance.GetDescription(entity);
                entitiesGroupsDescription = Strings.Join(entitiesGroups);
            }
            if (permissions.Length == 0)
            {
                var usersGroups = _authorizationRepository.GetAssociatedUsersGroupFor(user);

                if (entity == null)
                {
                    info.AddDeny("Разрешение на операцию '{0}' не было предоставлено пользователю '{1}' или группе '{1}', связанной с ('{2}')",
                                 operation,
                                 user.SecurityInfo.Name,
                                 Strings.Join(usersGroups)
                        );
                }
                else
                {
                    info.AddDeny("Разрешение на операцию '{0}' не было предоставлено пользователю '{1}' или группе '{1}', связанной с ('{2}'), для сущности '{3}' или группе сущностей '{3}', связанной с ('{4}')",
                                 operation,
                                 user.SecurityInfo.Name,
                                 Strings.Join(usersGroups),
                                 entityDescription,
                                 entitiesGroupsDescription);
                }
                return;
            }
            foreach (Permission permission in permissions)
            {
                AddUserLevelPermissionMessage(operation, info, user, permission, entityDescription,
                                              entitiesGroupsDescription);
                AddUserGroupLevelPermissionMessage(operation, info, user, permission, entityDescription,
                                                   entitiesGroupsDescription);
            }
        }

        private bool InitializeAuthorizationInfo(string operation, out AuthorizationInformation info)
        {
            info = new AuthorizationInformation();
            Operation op = _authorizationRepository.GetOperationByName(operation);
            if (op == null)
            {
                info.AddDeny("Операция '{0}' не определена", operation);
                return true;
            }
            return false;
        }

        private void AddUserGroupLevelPermissionMessage(string operation, AuthorizationInformation info,
                                                        User user, Permission permission,
                                                        string entityDescription,
                                                        string entitiesGroupsDescription)
        {
            if (permission.UsersGroup != null)
            {
                UsersGroup[] ancestryAssociation =
                    _authorizationRepository.GetAncestryAssociation(user, permission.UsersGroup.Name);
                string groupAncestry = Strings.Join(ancestryAssociation, " -> ");
                if (permission.Allow)
                {
                    info.AddAllow("Permission (level {4}) for operation '{0}' was granted to group '{1}' on '{2}' ('{3}' is a member of '{5}')",
                                  operation,
                                  permission.UsersGroup.Name,
                                  GetPermissionTarget(permission, entityDescription, entitiesGroupsDescription),
                                  user.SecurityInfo.Name,
                                  permission.Level,
                                  groupAncestry);
                }
                else
                {
                    info.AddDeny("Permission (level {4}) for operation '{0}' was denied to group '{1}' on '{2}' ('{3}' is a member of '{5}')",
                                 operation,
                                 permission.UsersGroup.Name,
                                 GetPermissionTarget(permission, entityDescription, entitiesGroupsDescription),
                                 user.SecurityInfo.Name,
                                 permission.Level,
                                 groupAncestry);
                }
            }
        }

        private static void AddUserLevelPermissionMessage(
            string operation,
            AuthorizationInformation info,
            User user,
            Permission permission,
            string entityDescription,
            string entitiesGroupsDescription)
        {
            if (permission.User != null)
            {
                string target = GetPermissionTarget(permission, entityDescription, entitiesGroupsDescription);
                if (permission.Allow)
                {
                    info.AddAllow("Permission (level {3}) for operation '{0}' was granted to '{1}' on '{2}'",
                                  operation,
                                  user.SecurityInfo.Name,
                                  target,
                                  permission.Level);
                }
                else
                {
                    info.AddDeny("Permission (level {3}) for operation '{0}' was denied to '{1}' on '{2}'",
                                 operation,
                                 user.SecurityInfo.Name,
                                 target,
                                 permission.Level);
                }
            }
        }

        private static string GetPermissionTarget(Permission permission, string entityDescription,
                                                  string entitiesGroupsDescription)
        {
            if (permission.EntitiesGroup != null)
            {
                if (string.IsNullOrEmpty(entitiesGroupsDescription) == false)
                {
                    return string.Format("'{0}' ('{1}' is a member of '{2}')",
                                         permission.EntitiesGroup.Name,
                                         entityDescription, entitiesGroupsDescription);
                }
                else
                {
                    return permission.EntitiesGroup.Name;
                }
            }
            if (permission.EntitySecurityKey != null)
                return entityDescription;
            return "Everything";
        }
    }
}
