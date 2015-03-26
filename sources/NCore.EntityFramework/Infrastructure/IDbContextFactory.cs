
namespace NCore.EntityFramework.Infrastructure
{
    public interface IDbContextFactory
    {
        IDbContext CreateDbContext();
    }
}
