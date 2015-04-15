using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IPermissionBuilder
    {
        Permission Save();
        Permission Build();
    }
}
