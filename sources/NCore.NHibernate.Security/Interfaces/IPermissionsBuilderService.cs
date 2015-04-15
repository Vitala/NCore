using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IPermissionsBuilderService
    {
        IForPermissionBuilder Allow(string operationName);
        IForPermissionBuilder Deny(string operationName);
        IForPermissionBuilder Allow(Operation operation);
        IForPermissionBuilder Deny(Operation operation);
    }
}
