using NCore.Security.Model;
using NCore.Security.NHibernate.Model;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IAuthorizationRepository
    {
        void RemoveUser(User user);

        UsersGroup CreateUsersGroup(string name);
        UsersGroup[] GetAssociatedUsersGroupFor(User user);
        UsersGroup GetUsersGroupByName(string groupName);
        UsersGroup RenameUsersGroup(string usersGroupName, string newName);
        void AssociateUserWith(User user, string groupName);
        void AssociateUserWith(User user, UsersGroup group);
        void DetachUserFromGroup(User user, string usersGroupName);
        UsersGroup CreateChildUserGroupOf(string parentGroupName, string usersGroupName);
        UsersGroup[] GetAncestryAssociation(User user, string usersGroupName);
        void RemoveUsersGroup(string usersGroupName);

        EntitiesGroup CreateEntitiesGroup(string name);
        EntitiesGroup[] GetAssociatedEntitiesGroupsFor<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>;
        EntitiesGroup GetEntitiesGroupByName(string groupName);
        EntitiesGroup RenameEntitiesGroup(string entitiesGroupName, string newName);
        void AssociateEntityWith<TEntity>(TEntity entity, string groupName) where TEntity : IEntityInformationExtractor<TEntity>;
        void AssociateEntityWith<TEntity>(TEntity entity, EntitiesGroup group) where TEntity : IEntityInformationExtractor<TEntity>;
        void DetachEntityFromGroup<TEntity>(TEntity entity, string entitiesGroupName) where TEntity : IEntityInformationExtractor<TEntity>;
        EntitiesGroup CreateChildEntityGroupOf(string parentGroupName, string usersGroupName);
        EntitiesGroup[] GetAncestryAssociationOfEntity<TEntity>(TEntity entity, string entityGroupName) where TEntity : IEntityInformationExtractor<TEntity>;
        void RemoveEntitiesGroup(string entitesGroupName);

        Operation CreateOperation(string operationName, string comment);
        Operation CreateOperation(string operationName);
        Operation GetOperationByName(string operationName);
        void RemoveOperation(string operationName);

        void SavePermission(Permission permission);
        void RemovePermission(Permission permission);
        Permission[] GetPermissionsFor(User user, bool eager = false);
        Permission[] GetPermissionsFor(UsersGroup usersGroup, bool eager = false);
        Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>;
        Permission[] GetGlobalPermissionsFor(User user, string operationName);
        Permission[] GetPermissionsFor(string operationName);
        Permission[] GetPermissionsFor<TEntity>(User user, TEntity entity, string operationName) where TEntity : IEntityInformationExtractor<TEntity>;
        Permission[] GetPermissionsFor<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>;
    }
}