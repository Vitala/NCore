
namespace NCore.Security.NHibernate.Interfaces
{
    public interface ILevelPermissionBuilder
    {
        IPermissionBuilder Level(int level);
        IPermissionBuilder DefaultLevel();
    }
}
