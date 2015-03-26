using System.Data.Entity;
using System.Reflection;
using System.Data;

namespace NCore.EntityFramework.Infrastructure
{
    public class EfDbContext : DbContext, IDbContext
    {
        private readonly Assembly _assemblyMapper;

        public EfDbContext(string connectionStringKey, Assembly assemblyMapper)
            : base(connectionStringKey)
        {
            _assemblyMapper = assemblyMapper;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //TODO: map all entity configurations from _assemblyMapper
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
           return Database.BeginTransaction(isolationLevel);
        }

        public DbContextTransaction BeginTransaction()
        {
           return Database.BeginTransaction();
        }
    }
}
