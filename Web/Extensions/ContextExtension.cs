using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Extensions;

public static class ContextExtension
{
    public static void MergeCollections<T, TKey>(this DbContext context, ICollection<T> currentItems, ICollection<T> newItems, Func<T, TKey> keyFunc) 
        where T : class
    {
        List<T> toRemove = null;
        foreach (var item in currentItems)
        {
            var currentKey = keyFunc(item);
            var found = newItems.FirstOrDefault(x => currentKey.Equals(keyFunc(x)));
            if (found == null)
            {
                toRemove ??= new List<T>();
                toRemove.Add(item);
            }
            else
            {
                if (!ReferenceEquals(found, item))
                    context.Entry(item).CurrentValues.SetValues(found);
            }
        }

        if (toRemove != null)
        {
            foreach (var item in toRemove)
            {
                currentItems.Remove(item);
                context.Set<T>().Remove(item);
            }
        }

        foreach (var newItem in newItems)
        {
            var newKey = keyFunc(newItem);
            var found = currentItems.FirstOrDefault(x => newKey.Equals(keyFunc(x)));
            if (found == null)
            {
                currentItems.Add(newItem);
            }
        }
    }
}