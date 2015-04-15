using System.Collections.Generic;

namespace NCore.NHibernate.Security.Helpers
{
    public static class Strings
    {
        public static string GetParentOperationName(string operationName)
        {
            var lastIndex = operationName.LastIndexOf('/');
            return operationName.Substring(0, lastIndex);
        }

        public static string[] GetHierarchicalOperationNames(string operationName)
        {
            var names = new List<string>();
            do
            {
                names.Add(operationName);
                operationName = GetParentOperationName(operationName);
            } while (operationName != "");
            return names.ToArray();
        }
    }
}
