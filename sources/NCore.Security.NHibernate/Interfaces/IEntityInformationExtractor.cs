using System;

namespace NCore.Security.NHibernate.Interfaces
{
    public interface IEntityInformationExtractor<TEntity>
    {
        Guid SecurityKey { get; }
    }
}
