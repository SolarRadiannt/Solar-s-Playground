using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static bool TryFirst<T>(this IEnumerable<T> source, out T result)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        if (source is IList<T> list)
            if (list.Count > 0)
            {
                result = list[0];
                return true;
            }
        
        
        foreach (var item in source)
        {
            result = item;
            return true;
        }

        result = default;
        return false;
    }
}