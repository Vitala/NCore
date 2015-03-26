
namespace NCore.EntityFramework.Infrastructure
{
    public interface ICurrentDbContextProvider
    {
        IDbContext CurrentContext { get; }
    }
}
