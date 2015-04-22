using NCore.Security.NHibernate.Model;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IPermissionsBuilderService
    {
        IForPermissionBuilder Allow(string operationName);
        IForPermissionBuilder Deny(string operationName);
        IForPermissionBuilder Allow(Operation operation);
        IForPermissionBuilder Deny(Operation operation);
    }
}
