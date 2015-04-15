using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using NCore.NHibernate.Security.Helpers;

namespace NCore.NHibernate.Security.Services
{
    //HACK: диспозабл не нужен, сессия управляется из вне
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly ISession _session;

        public AuthorizationRepository(ICurrentSessionProvider currentSessionProvider)
        {
            _session = currentSessionProvider.CurrentSession;
            /* HACK:
            if (_session == null)
                throw new NCoreException("Невозможно использовать репозиторий без контекста юнит-оф-ворк. Откройте новый юнит-оф-ворк перед созданием репозитория.");
             */
        }

        #region Группы пользователей

        private static List<UsersGroup> Min(List<UsersGroup> first, List<UsersGroup> second)
        {
            if (first.Count == 0)
                return second;
            if (first.Count <= second.Count)
                return first;
            return second;
        }

        public void RemoveUser(User user)
        {
            ICollection<UsersGroup> groups =
                SecurityCriterions.DirectUsersGroups((user))
                    .GetExecutableCriteria(_session)
                    .SetCacheable(true)
                    .List<UsersGroup>();

            foreach (UsersGroup group in groups)
            {
                group.Users.Remove(user);
            }

            _session.CreateQuery("delete Permission p where p.User = :user")
                .SetEntity("user", user)
                .ExecuteUpdate();
        }

        public virtual UsersGroup CreateUsersGroup(string name)
        {
            var ug = new UsersGroup { Name = name };
            _session.Save(ug);
            return ug;
        }

        public virtual UsersGroup CreateChildUserGroupOf(string parentGroupName, string usersGroupName)
        {
            var parent = GetUsersGroupByName(parentGroupName);
            if (parent == null)
                throw new ArgumentException(String.Format("Родительская группа пользователей '{0}' не существует", parentGroupName));

            var group = CreateUsersGroup(usersGroupName);
            group.Parent = parent;
            group.AllParents.AddAll(parent.AllParents);
            group.AllParents.Add(parent);
            parent.DirectChildren.Add(group);
            parent.AllChildren.Add(group);
            return group;
        }

        public virtual void RemoveUsersGroup(string usersGroupName)
        {
            var group = GetUsersGroupByName(usersGroupName);
            if (group == null)
                return;

            if (group.DirectChildren.Any())
                throw new InvalidOperationException(String.Format("Невозможно удалить группу пользователей '{0}', пока у нее есть подгруппы.", usersGroupName));

            _session.CreateQuery("delete Permission p where p.UsersGroup = :group")
                .SetEntity("group", group)
                .ExecuteUpdate();

            if (group.Parent != null)
            {
                group.Parent.DirectChildren.Remove(group);
            }
            foreach (var parent in group.AllParents)
            {
                parent.AllChildren.Remove(group);
            }
            group.AllParents.Clear();
            group.Users.Clear();

            _session.Delete(group);
        }

        public virtual UsersGroup RenameUsersGroup(string usersGroupName, string newName)
        {
            var group = GetUsersGroupByName(usersGroupName);
            if (group == null) throw new InvalidOperationException(String.Format("Группа пользователей '{0}' не найдена", usersGroupName));
            group.Name = newName;

            _session.Save(group);
            return group;
        }

        public virtual UsersGroup[] GetAncestryAssociation(User user, string usersGroupName)
        {
            var desiredGroup = GetUsersGroupByName(usersGroupName);
            ICollection<UsersGroup> directGroups =
                SecurityCriterions.DirectUsersGroups((user))
                    .GetExecutableCriteria(_session)
                    .SetCacheable(true)
                    .List<UsersGroup>();

            if (directGroups.Contains(desiredGroup))
            {
                return new[] { desiredGroup };
            }

            var associatedGroups = GetAssociatedUsersGroupFor(user);
            if (!associatedGroups.Contains(desiredGroup))
            {
                return new UsersGroup[0];
            }

            var shortest = new List<UsersGroup>();
            foreach (var usersGroup in associatedGroups)
            {
                var path = new List<UsersGroup>();
                var current = usersGroup;
                while (current != null && current != desiredGroup)
                {
                    path.Add(current);
                    current = current.Parent;
                }
                if (current != null)
                    path.Add(current);

                if (path.Contains(desiredGroup) && directGroups.Contains(path[0]))
                {
                    shortest = Min(shortest, path);
                }
            }
            return shortest.ToArray();
        }

        public virtual UsersGroup[] GetAssociatedUsersGroupFor(User user)
        {
            var usersGroups =
                SecurityCriterions.AllGroups(user)
                    .GetExecutableCriteria(_session)
                    .AddOrder(Order.Asc("Name"))
                    .SetCacheable(true)
                    .List<UsersGroup>();
            return usersGroups.ToArray();
        }

        public virtual UsersGroup GetUsersGroupByName(string groupName)
        {
            return _session.CreateCriteria<UsersGroup>()
                .Add(Restrictions.Eq("Name", groupName))
                .SetCacheable(true)
                .UniqueResult<UsersGroup>();
        }

        public virtual void AssociateUserWith(User user, string groupName)
        {
            var group = GetUsersGroupByName(groupName);
            if (group == null)
                throw new InvalidOperationException(String.Format("Группа пользователей '{0}' не найдена", groupName));

            AssociateUserWith(user, group);
        }

        public void AssociateUserWith(User user, UsersGroup group)
        {
            group.Users.Add(user);
        }

        public void DetachUserFromGroup(User user, string usersGroupName)
        {
            var group = GetUsersGroupByName(usersGroupName);
            if (group == null)
                throw new InvalidOperationException(String.Format("Группа пользователей '{0}' не найдена", usersGroupName));

            group.Users.Remove(user);
        }

        #endregion

        #region Группы сущностей

        private static List<EntitiesGroup> Min(List<EntitiesGroup> first, List<EntitiesGroup> second)
        {
            if (first.Count == 0)
                return second;
            if (first.Count <= second.Count)
                return first;
            return second;
        }

        public virtual EntitiesGroup CreateChildEntityGroupOf(string parentGroupName, string usersGroupName)
        {
            var parent = GetEntitiesGroupByName(parentGroupName);
            if (parent == null)
                throw new ArgumentException(String.Format("Родительская группа пользователей '{0}' не существует", parentGroupName));

            var group = CreateEntitiesGroup(usersGroupName);
            group.Parent = parent;
            group.AllParents.AddAll(parent.AllParents);
            group.AllParents.Add(parent);
            parent.DirectChildren.Add(group);
            parent.AllChildren.Add(group);

            return group;
        }

        public virtual void RemoveEntitiesGroup(string entitesGroupName)
        {
            var group = GetEntitiesGroupByName(entitesGroupName);
            if (group == null)
                return;
            if (group.DirectChildren.Any())
                throw new InvalidOperationException(String.Format("Невозможно удалить группу сущностей '{0}', пока у нее есть подгруппы", entitesGroupName));

            _session.CreateQuery("delete Permission p where p.EntitiesGroup = :group")
                .SetEntity("group", group)
                .ExecuteUpdate();

            if (group.Parent != null)
            {
                group.Parent.DirectChildren.Remove(group);
            }
            foreach (var parent in group.AllParents)
            {
                parent.AllChildren.Remove(group);
            }
            group.AllParents.Clear();
            group.Entities.Clear();

            _session.Delete(group);
        }

        public virtual EntitiesGroup[] GetAncestryAssociationOfEntity<TEntity>(TEntity entity, string entityGroupName) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var desiredGroup = GetEntitiesGroupByName(entityGroupName);
            ICollection<EntitiesGroup> directGroups =
                SecurityCriterions.DirectEntitiesGroups(entity)
                    .GetExecutableCriteria(_session)
                    .SetCacheable(true)
                    .List<EntitiesGroup>();

            if (directGroups.Contains(desiredGroup))
            {
                return new[] { desiredGroup };
            }
            var associatedGroups = GetAssociatedEntitiesGroupsFor(entity);
            if (!associatedGroups.Contains(desiredGroup))
            {
                return new EntitiesGroup[0];
            }
            var shortest = new List<EntitiesGroup>();
            foreach (var entitiesGroup in associatedGroups)
            {
                var path = new List<EntitiesGroup>();
                var current = entitiesGroup;
                while (current != null && current != desiredGroup)
                {
                    path.Add(current);
                    current = current.Parent;
                }
                if (current != null)
                    path.Add(current);

                if (path.Contains(desiredGroup) && directGroups.Contains(path[0]))
                {
                    shortest = Min(shortest, path);
                }
            }
            return shortest.ToArray();
        }

        public virtual EntitiesGroup CreateEntitiesGroup(string name)
        {
            var eg = new EntitiesGroup { Name = name };
            _session.Save(eg);
            return eg;
        }

        public virtual EntitiesGroup RenameEntitiesGroup(string entitiesGroupName, string newName)
        {
            var group = GetEntitiesGroupByName(entitiesGroupName);
            if (group == null)
                throw new InvalidOperationException(String.Format("Группа сущностей '{0}' не найдена", entitiesGroupName));
            group.Name = newName;
            _session.Save(group);
            return group;
        }

        public virtual EntitiesGroup GetEntitiesGroupByName(string groupName)
        {
            return _session.CreateCriteria<EntitiesGroup>()
                .Add(Restrictions.Eq("Name", groupName))
                .SetCacheable(true)
                .UniqueResult<EntitiesGroup>();
        }

        public virtual EntitiesGroup[] GetAssociatedEntitiesGroupsFor<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var entitiesGroups =
                SecurityCriterions.AllGroups(entity)
                    .GetExecutableCriteria(_session)
                    .AddOrder(Order.Asc("Name"))
                    .SetCacheable(true)
                    .List<EntitiesGroup>();

            return entitiesGroups.ToArray();
        }

        public virtual void AssociateEntityWith<TEntity>(TEntity entity, string groupName) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var entitiesGroup = GetEntitiesGroupByName(groupName);
            if (entitiesGroup == null)
                throw new ArgumentException(String.Format("Группа сущностей '{0}' не найдена", groupName));

            AssociateEntityWith(entity, entitiesGroup);
        }

        public void AssociateEntityWith<TEntity>(TEntity entity, EntitiesGroup entitiesGroup) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var key = entity.SecurityKey;

            var reference = GetOrCreateEntityReference<TEntity>(key);
            entitiesGroup.Entities.Add(reference);
        }

        public void DetachEntityFromGroup<TEntity>(TEntity entity, string entitiesGroupName) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var entitiesGroup = GetEntitiesGroupByName(entitiesGroupName);
            if (entitiesGroup == null)
                throw new InvalidOperationException(String.Format("Группа сущностей '{0}' не найдена", entitiesGroupName));

            var key = entity.SecurityKey;

            EntityReference reference = GetOrCreateEntityReference<TEntity>(key);
            entitiesGroup.Entities.Remove(reference);
        }

        private EntityReference GetOrCreateEntityReference<TEntity>(Guid key)
        {
            EntityReference reference = _session.CreateCriteria<EntityReference>()
                .Add(Restrictions.Eq("EntitySecurityKey", key))
                .SetCacheable(true)
                .UniqueResult<EntityReference>();
            if (reference == null)
            {
                reference = new EntityReference();
                reference.EntitySecurityKey = key;
                reference.Type = GetOrCreateEntityType<TEntity>();
                _session.Save(reference);
            }
            return reference;
        }

        private EntityType GetOrCreateEntityType<TEntity>()
        {
            EntityType entityType = _session.CreateCriteria<EntityType>()
                .Add(Restrictions.Eq("Name", typeof(TEntity).FullName))
                .SetCacheable(true)
                .UniqueResult<EntityType>();
            if (entityType == null)
            {
                entityType = new EntityType { Name = typeof(TEntity).FullName };
                _session.Save(entityType);
            }
            return entityType;
        }

        #endregion

        #region Операции

        public virtual void RemoveOperation(string operationName)
        {
            var operation = GetOperationByName(operationName);
            if (operation == null)
                return;

            if (operation.Children.Any())
                throw new InvalidOperationException(String.Format("Невозможно удалить операцию '{0}' пока у нее есть дочерние операции", operationName));

            _session.CreateQuery("delete Permission p where p.Operation = :operation")
                .SetEntity("operation", operation)
                .ExecuteUpdate();

            if (operation.Parent != null)
            {
                operation.Parent.Children.Remove(operation);
            }

            _session.Delete(operation);
        }

        public virtual Operation CreateOperation(string operationName)
        {
            if (string.IsNullOrEmpty(operationName))
                throw new ArgumentException("Имя операции не указано");
            if (operationName[0] != '/')
                throw new ArgumentException("Имя операции должно начинаться с '/'");

            var op = new Operation { Name = operationName };

            var parentOperationName = Strings.GetParentOperationName(operationName);
            if (parentOperationName != string.Empty)
            {
                var parentOperation = GetOperationByName(parentOperationName);
                if (parentOperation == null)
                    parentOperation = CreateOperation(parentOperationName);

                op.Parent = parentOperation;
                parentOperation.Children.Add(op);
            }

            _session.Save(op);
            return op;
        }

        public virtual Operation GetOperationByName(string operationName)
        {
            return _session.CreateCriteria<Operation>()
             .Add(Restrictions.Eq("Name", operationName))
             .SetCacheable(true)
             .UniqueResult<Operation>();
        }

        #endregion

        #region Разрешения

        public void RemovePermission(Permission permission)
        {
            _session.Delete(permission);
        }

        public virtual void SavePermission(Permission permission)
        {
            _session.Save(permission);
        }

        public Permission[] GetPermissionsFor(User user)
        {
            var criteria = DetachedCriteria.For<Permission>()
                .Add(Expression.Eq("User", user)
                     || Subqueries.PropertyIn("UsersGroup.Id",
                                              SecurityCriterions.AllGroups(user).SetProjection(Projections.Id())));

            return FindResults(criteria);
        }

        public Permission[] GetGlobalPermissionsFor(User user, string operationName)
        {
            var operationNames = Strings.GetHierarchicalOperationNames(operationName);
            var criteria = DetachedCriteria.For<Permission>()
                .Add(Expression.Eq("User", user)
                     || Subqueries.PropertyIn("UsersGroup.Id",
                                              SecurityCriterions.AllGroups(user).SetProjection(Projections.Id())))
                .Add(Expression.IsNull("EntitiesGroup"))
                .Add(Expression.IsNull("EntitySecurityKey"))
                .CreateAlias("Operation", "op")
                .Add(Expression.In("op.Name", operationNames));

            return FindResults(criteria);
        }

        public Permission[] GetPermissionsFor(string operationName)
        {
            var operationNames = Strings.GetHierarchicalOperationNames(operationName);
            DetachedCriteria criteria = DetachedCriteria.For<Permission>()
                .CreateAlias("Operation", "op")
                .Add(Restrictions.In("op.Name", operationNames));

            return FindResults(criteria);
        }

        public Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var key = entity.SecurityKey;
            var entitiesGroups = GetAssociatedEntitiesGroupsFor(entity);

            var criteria = DetachedCriteria.For<Permission>()
                .Add(Expression.Eq("User", user)
                     || Subqueries.PropertyIn("UsersGroup.Id",
                                              SecurityCriterions.AllGroups(user).SetProjection(Projections.Id())))
                .Add(Expression.Eq("EntitySecurityKey", key) || Expression.In("EntitiesGroup", entitiesGroups));

            return FindResults(criteria);
        }

        public Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity, string operationName) where TEntity : IEntityInformationExtractor<TEntity>
        {
            var key = entity.SecurityKey;
            var operationNames = Strings.GetHierarchicalOperationNames(operationName);
            var entitiesGroups = GetAssociatedEntitiesGroupsFor(entity);

            //var usersGroups = GetAssociatedUsersGroupFor(user);					

            var onCriteria =
                (Restrictions.Eq("EntitySecurityKey", key) || Restrictions.In("EntitiesGroup", entitiesGroups)) ||
                (Restrictions.IsNull("EntitiesGroup") && Restrictions.IsNull("EntitySecurityKey"));

            var criteria = DetachedCriteria.For<Permission>()
                .Add(Restrictions.Eq("User", user)
                     || Subqueries.PropertyIn("UsersGroup.Id",
                                              SecurityCriterions.AllGroups(user).SetProjection(Projections.Id())))
                .Add(onCriteria)
                .CreateAlias("Operation", "op")
                .Add(Restrictions.In("op.Name", operationNames));

            return FindResults(criteria);
        }

        public Permission[] GetPermissionsFor<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>
        {
            if (entity is User)
                return GetPermissionsFor(entity as User);

            var key = entity.SecurityKey;
            var groups = GetAssociatedEntitiesGroupsFor(entity);
            var criteria = DetachedCriteria.For<Permission>()
                .Add(Expression.Eq("EntitySecurityKey", key) || Expression.In("EntitiesGroup", groups));

            return FindResults(criteria);
        }

        private Permission[] FindResults(DetachedCriteria criteria)
        {
            ICollection<Permission> permissions = criteria.GetExecutableCriteria(_session)
                .AddOrder(Order.Desc("Level"))
                .AddOrder(Order.Asc("Allow"))
                .SetCacheable(true)
                .List<Permission>();
            return permissions.ToArray();
        }

        #endregion
    }
}
