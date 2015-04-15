using System;

namespace NCore.NHibernate.Security.Interfaces
{
    public interface IEntityInformationExtractor<TEntity>
    {
        Guid SecurityKey { get; }
    }
}
