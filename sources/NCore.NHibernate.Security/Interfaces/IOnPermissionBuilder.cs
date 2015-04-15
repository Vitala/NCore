using NCore.NHibernate.Security.Model;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IOnPermissionBuilder
    {
        ILevelPermissionBuilder On<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>;
        ILevelPermissionBuilder On(EntitiesGroup entity);
        ILevelPermissionBuilder On(string entitiesGroupName);
        ILevelPermissionBuilder OnEverything();
    }
}
