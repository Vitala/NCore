using System.Reflection;

namespace NCore.EntityFramework.Infrastructure
{
    public class DbContextFactory : IDbContextFactory
    {
        internal string ConnectionString { get; set; }
        internal Assembly AssemblyMapper { get; set; }

        public IDbContext CreateDbContext()
        {
            return new EfDbContext(ConnectionString, AssemblyMapper);
        }
    }
}
