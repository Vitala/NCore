using System;
using System.Reflection;

namespace NCore.EntityFramework.Infrastructure
{
    public class DbContextFactoryBuilder
    {
        private string _connectionString;
        private string _connectionStringName;
        private Assembly _assemblyMapper;

        public static DbContextFactoryBuilder Create()
        {
            return new DbContextFactoryBuilder();
        }

        public DbContextFactoryBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            _connectionStringName = null;
            return this;
        }

        public DbContextFactoryBuilder WithConnectionStringName(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
            _connectionString = null;
            return this;
        }

        public DbContextFactoryBuilder MapEntitiesFrom(Assembly assemblyMapper)
        {
            _assemblyMapper = assemblyMapper;
            return this;
        }

        public DbContextFactory Build()
        {
            return new DbContextFactory()
            {
                ConnectionString = _connectionString ?? _connectionStringName,
                AssemblyMapper = _assemblyMapper
            };
        }

    }
}
