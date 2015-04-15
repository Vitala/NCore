using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
using System;

namespace NCore.NHibernate.Security.Services
{
    public class PermissionsBuilderService : IPermissionsBuilderService
    {
        private readonly IAuthorizationRepository _authorizationRepository;

        public PermissionsBuilderService(IAuthorizationRepository authorizationRepositoryFactory)
        {
            _authorizationRepository = authorizationRepositoryFactory;
        }

        public class FluentPermissionBuilder : IPermissionBuilder, IForPermissionBuilder, IOnPermissionBuilder,
                                               ILevelPermissionBuilder
        {
            private readonly Permission _permission = new Permission();
            private readonly PermissionsBuilderService _permissionBuilderService;

            public FluentPermissionBuilder(PermissionsBuilderService permissionBuilderService, bool allow, Operation operation)
            {
                _permissionBuilderService = permissionBuilderService;
                _permission.Allow = allow;
                _permission.Operation = operation;
            }

            public Permission Save()
            {
                _permissionBuilderService.Save(_permission);
                return _permission;
            }

            public Permission Build()
            {
                return _permission;
            }

            public IOnPermissionBuilder For(User user)
            {
                _permission.User = user;
                return this;
            }

            public IOnPermissionBuilder For(string usersGroupName)
            {
                var usersGroup = _permissionBuilderService
                    ._authorizationRepository
                    .GetUsersGroupByName(usersGroupName);

                if (usersGroup == null)
                    throw new ArgumentException(String.Format("Группа пользователей '{0}' не найдена", usersGroupName));

                return For(usersGroup);
            }

            public IOnPermissionBuilder For(UsersGroup usersGroup)
            {
                _permission.UsersGroup = usersGroup;
                return this;
            }

             public ILevelPermissionBuilder On<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>
            {
                _permission.SetEntityType(typeof(TEntity));
                _permission.EntitySecurityKey = entity.SecurityKey;
                return this;
            }

             public ILevelPermissionBuilder On(string entitiesGroupName)
             {
                 var entitiesGroup =
                     _permissionBuilderService
                         ._authorizationRepository
                         .GetEntitiesGroupByName(entitiesGroupName);
                 if (entitiesGroup == null)
                     throw new ArgumentException(String.Format("Группа сущностей '{0}' не найдена", entitiesGroupName));
                 return On(entitiesGroup);

             }

            public ILevelPermissionBuilder On(EntitiesGroup entitiesGroup)
            {
                _permission.EntitiesGroup = entitiesGroup;
                return this;
            }

            public ILevelPermissionBuilder OnEverything()
            {
                return this;
            }

            public IPermissionBuilder Level(int level)
            {
                _permission.Level = level;
                return this;
            }

            public IPermissionBuilder DefaultLevel()
            {
                return Level(1);
            }
        }

        public void Save(Permission permission)
        {
            _authorizationRepository.SavePermission(permission);
        }

        public IForPermissionBuilder Allow(string operationName)
        {
            var operation = _authorizationRepository.GetOperationByName(operationName);
            if (operation == null)
                throw new ArgumentException(String.Format("Операция '{0}' не найдена", operationName));
            return Allow(operation);
        }

        public IForPermissionBuilder Deny(string operationName)
        {
            var operation = _authorizationRepository.GetOperationByName(operationName);
            if (operation == null)
                throw new ArgumentException(String.Format("Операция '{0}' не найдена", operationName));
            return Deny(operation);
        }

        public IForPermissionBuilder Allow(Operation operation)
        {
            return new FluentPermissionBuilder(this, true, operation);
        }

        public IForPermissionBuilder Deny(Operation operation)
        {
            return new FluentPermissionBuilder(this, false, operation);
        }
    }
}
