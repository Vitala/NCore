﻿using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
using NCore.NHibernate.Security.Helpers;
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

        public bool IsAllowed<TEntity>(User user, TEntity entity, string operation) where TEntity : IEntityInformationExtractor<TEntity>
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
            return criteria.Alias + ".SecurityKey";
        }

        private string GetSecurityKeyProperty(ICriteria criteria)
        {
            var rootType = criteria.GetRootEntityTypeIfAvailable();
            return criteria.Alias + ".SecurityKey";
        }

    }
}
