using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static bool TryFirst<T>(this IEnumerable<T> source, out T result)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        foreach (var item in source)
        {
            result = item;
            return true;
        }

        result = default;
        return false;
    }
}