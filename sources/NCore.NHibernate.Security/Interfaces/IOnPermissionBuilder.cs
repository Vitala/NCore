using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IOnPermissionBuilder
    {
        ILevelPermissionBuilder On<TEntity>(TEntity entity) where TEntity : class;
        ILevelPermissionBuilder On(EntitiesGroup entity);
        ILevelPermissionBuilder On(string entitiesGroupName);
        ILevelPermissionBuilder OnEverything();
    }
}
