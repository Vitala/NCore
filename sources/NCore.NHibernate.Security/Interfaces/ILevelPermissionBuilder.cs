
namespace NCore.NHibernate.Security.Interfaces
{
    public interface ILevelPermissionBuilder
    {
        IPermissionBuilder Level(int level);
        IPermissionBuilder DefaultLevel();
    }
}
