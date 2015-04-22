using NCore.Security.Model;
using NCore.Security.NHibernate.Model;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using System;

namespace NCore.Security.NHibernate
{
    public class UserMapper
    {
        private readonly Configuration _cfg;
        private readonly Type _userType;
        private bool _performedMapping = false;

        public UserMapper(Configuration cfg, Type userType)
        {
            _cfg = cfg;
            _userType = userType;
        }

        public void Map()
        {
            var dialect = Dialect.GetDialect(_cfg.Properties);
            var mappings = _cfg.CreateMappings(dialect);
            mappings.AddSecondPass(delegate { PerformMapping(); });
        }

        private void PerformMapping()
        {
            if (_performedMapping)
                return;
            _performedMapping = true;

            var classMapping = GetUserMapping();

            foreach (var persistentClass in _cfg.ClassMappings)
            {
                if (persistentClass.MappedClass.Assembly != typeof(UserMap).Assembly)
                    continue;

                foreach (var property in persistentClass.PropertyIterator)
                {
                    HandleManyToOne(property, classMapping);

                    HandleManyToManySet(property, persistentClass);
                }
                UpdateForeignKeyReferences(classMapping, persistentClass.Table);
            }
        }

        private PersistentClass GetUserMapping()
        {
            var classMapping = _cfg.GetClassMapping(_userType);
            if (classMapping == null)
            {
                throw new InvalidOperationException(String.Format("Невозможно найти маппинг для класса пользователя {0}", _userType));
            }
            return classMapping;
        }

        private void UpdateForeignKeyReferences(PersistentClass classMapping, Table table)
        {
            foreach (var key in table.ForeignKeyIterator)
            {
                if (key.ReferencedEntityName != typeof(User).FullName)
                    continue;

                key.ReferencedEntityName = _userType.FullName;
                key.ReferencedTable = classMapping.Table;
            }
        }

        private void HandleManyToManySet(Property property, PersistentClass classMapping)
        {
            var collection = property.Value as Collection;
            if (collection == null || collection.IsOneToMany ||
                collection.GenericArguments.Length != 1 ||
                collection.GenericArguments[0] != typeof(User))
                return;
            UpdateForeignKeyReferences(classMapping, collection.Element.Table);
            var one = collection.Element as ManyToOne;
            if (one != null)
            {
                var element = new ManyToOne(one.Table) { 
                    IsLazy = one.IsLazy,
                    ReferencedEntityName = _userType.FullName,
                    ReferencedPropertyName = one.ReferencedPropertyName,
                    IsIgnoreNotFound = one.IsIgnoreNotFound
                };
                CopyColumns(one, element);
                collection.Element = element;
            }
        }

        private void HandleManyToOne(Property property, PersistentClass classMapping)
        {
            var manyToOne = property.Value as ManyToOne;
            if (manyToOne == null || manyToOne.ReferencedEntityName != typeof(User).FullName)
                return;
            var value = new ManyToOne(classMapping.Table) { ReferencedEntityName = _userType.FullName };
            CopyColumns(manyToOne, value);
            property.Value = value;
        }

        private void CopyColumns(ManyToOne src, ManyToOne dest)
        {
            foreach (var selectable in src.ColumnIterator)
            {
                var col = selectable as Column;
                if (col != null)
                {
                    dest.AddColumn(col);
                }
            }
        }
    }
}
