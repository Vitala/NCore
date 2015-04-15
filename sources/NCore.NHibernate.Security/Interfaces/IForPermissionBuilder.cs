using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IForPermissionBuilder
    {
        IOnPermissionBuilder For(User user);
        IOnPermissionBuilder For(UsersGroup group);
        IOnPermissionBuilder For(string usersGroupName);
    }
}
