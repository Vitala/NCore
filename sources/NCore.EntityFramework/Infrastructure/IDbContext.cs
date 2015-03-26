using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NCore.EntityFramework.Infrastructure
{
    public interface IDbContext : IDisposable
    {
        int SaveChanges();
        DbContextTransaction BeginTransaction();
        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);
    }
}
