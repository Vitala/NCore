using System;

namespace NCore.Security.Model
{
    public interface IEntityInformationExtractor<TEntity>
    {
        Guid SecurityKey { get; }
    }
}
