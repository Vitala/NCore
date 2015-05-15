using NCore.Security.Model;
using NCore.Security.NHibernate.Model;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IOnPermissionBuilder
    {
        ILevelPermissionBuilder On<TEntity>(TEntity entity) where TEntity : IEntityInformationExtractor<TEntity>;
        ILevelPermissionBuilder On(EntitiesGroup entity);
        ILevelPermissionBuilder On(string entitiesGroupName);
        ILevelPermissionBuilder OnEverything();
    }
}
