using System.Collections.Generic;

namespace NCore.Security.NHibernate.Helpers
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
