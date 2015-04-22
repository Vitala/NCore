using NCore.Security.NHibernate.Model;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IPermissionBuilder
    {
        Permission Save();
        Permission Build();
    }
}
