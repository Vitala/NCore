using System;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IEntityInformationExtractor<TEntity>
    {
        Guid GetSecurityKeyFor(TEntity entity);
        string GetDescription(Guid securityKey);
        string SecurityKeyPropertyName { get; }
    }
}
