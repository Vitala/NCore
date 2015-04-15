using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IAuthorizationRepository
    {
        UsersGroup CreateUsersGroup(string name);
        EntitiesGroup CreateEntitiesGroup(string name);
        UsersGroup[] GetAssociatedUsersGroupFor(User user);
        UsersGroup GetUsersGroupByName(string groupName);
        EntitiesGroup GetEntitiesGroupByName(string groupName);
        EntitiesGroup[] GetAssociatedEntitiesGroupsFor<TEntity>(TEntity entity) where TEntity : class;
        void AssociateEntityWith<TEntity>(TEntity entity, string groupName) where TEntity : class;
        void AssociateEntityWith<TEntity>(TEntity entity, EntitiesGroup group) where TEntity : class;
        void AssociateUserWith(User user, string groupName);
        void AssociateUserWith(User user, UsersGroup group);
        Operation CreateOperation(string operationName);
        Operation GetOperationByName(string operationName);
        void DetachUserFromGroup(User user, string usersGroupName);
        void DetachEntityFromGroup<TEntity>(TEntity entity, string entitiesGroupName)
            where TEntity : class;
        UsersGroup CreateChildUserGroupOf(string parentGroupName, string usersGroupName);
        EntitiesGroup CreateChildEntityGroupOf(string parentGroupName, string usersGroupName);
        UsersGroup[] GetAncestryAssociation(User user, string usersGroupName);
        EntitiesGroup[] GetAncestryAssociationOfEntity<TEntity>(TEntity entity, string entityGroupName) where TEntity : class;
        void RemoveUsersGroup(string usersGroupName);
        void RemoveEntitiesGroup(string entitesGroupName);
        void RemoveOperation(string operationName);
        void RemoveUser(User user);
        void RemovePermission(Permission permission);
        UsersGroup RenameUsersGroup(string usersGroupName, string newName);
        EntitiesGroup RenameEntitiesGroup(string entitiesGroupName, string newName);
        void SavePermission(Permission permission);
        Permission[] GetPermissionsFor(User user);
        Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity) where TEntity : class;
        Permission[] GetGlobalPermissionsFor(User user, string operationName);
        Permission[] GetPermissionsFor(string operationName);
        Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity, string operationName) where TEntity : class;
        Permission[] GetPermissionsFor<TEntity>(TEntity entity) where TEntity : class;
    }
}