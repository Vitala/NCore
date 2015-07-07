using NHibernate.Dialect;
using System.Data;

namespace NCore.NHibernate.Postgre
{
    public class CustomPostgreSQLDialect : PostgreSQL82Dialect
    {
        public CustomPostgreSQLDialect()
        {
            RegisterColumnType(DbType.Object, "json");
        }
    }
}
