using System.Collections.Generic;

namespace NCore.NHibernate.Security
{
    internal static class CollectionExtensions
    {
        internal static void AddAll<T>(this ICollection<T> self, IEnumerable<T> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                self.Add(item);
            }
        }
    }
}
