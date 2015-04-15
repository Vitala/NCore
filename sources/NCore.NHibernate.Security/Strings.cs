using NCore.NHibernate.Security.Model;
using System.Collections.Generic;
using System.Text;

namespace NCore.NHibernate.Security
{
    public static class Strings
    {
        public static string GetParentOperationName(string operationName)
        {
            int lastIndex = operationName.LastIndexOf('/');
            return operationName.Substring(0, lastIndex);
        }

        public static string[] GetHierarchicalOperationNames(string operationName)
        {
            List<string> names = new List<string>();
            do
            {
                names.Add(operationName);
                operationName = GetParentOperationName(operationName);
            } while (operationName != "");
            return names.ToArray();
        }
        
        public static string Join(NamedEntity[] entities, string separator)
        {
            var sb = new StringBuilder();
            foreach (var entity in entities)
            {
                sb.Append(entity.Name).Append(separator);
            }
            if (sb.Length == 0)
                return "Без группы";
            sb.Remove(sb.Length - separator.Length, separator.Length);
            return sb.ToString();
        }
        
        public static string Join(NamedEntity[] entities)
        {
            return Join(entities, ", ");
        }
         
    }
}
