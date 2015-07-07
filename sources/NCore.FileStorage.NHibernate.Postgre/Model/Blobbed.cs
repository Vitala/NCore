using Newtonsoft.Json;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;

namespace NCore.FileStorage.NHibernate.Postgre
{
    [Serializable]
    public class Blobbed<T> : IUserType where T : class
    {
        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            var xdocX = JsonConvert.SerializeObject(x);
            var xdocY = JsonConvert.SerializeObject(y);

            return xdocY == xdocX;
        }

        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            if (names.Length != 1)
                throw new InvalidOperationException("Only expecting one column...");

            var val = rs[names[0]] as string;

            if (val != null && !string.IsNullOrWhiteSpace(val))
                return JsonConvert.DeserializeObject<T>(val);

            return null;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var parameter = (DbParameter)cmd.Parameters[index];

            if (value == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = JsonConvert.SerializeObject(value);
        }

        public object DeepCopy(object value)
        {
            if (value == null)
                return null;

            var serialized = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            var str = cached as string;
            return string.IsNullOrWhiteSpace(str) ? null : JsonConvert.DeserializeObject<T>(str);
        }

        public object Disassemble(object value)
        {
            return value == null ? null : JsonConvert.SerializeObject(value);
        }

        public SqlType[] SqlTypes
        {
            //we must write extended SqlType and return it here
            get
            {
                return new SqlType[] { new NpgsqlExtendedSqlType(DbType.Object, NpgsqlTypes.NpgsqlDbType.Json) };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(T); }
        }

        public bool IsMutable
        {
            get { return true; }
        }
    }
}
