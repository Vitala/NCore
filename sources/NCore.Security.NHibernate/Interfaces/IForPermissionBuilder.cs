using NCore.Security.Model;
using NCore.Security.NHibernate.Model;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IForPermissionBuilder
    {
        IOnPermissionBuilder For(User user);
        IOnPermissionBuilder For(UsersGroup group);
        IOnPermissionBuilder For(string usersGroupName);
    }
}
