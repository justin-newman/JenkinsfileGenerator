using System.Collections.Generic;

public static class ListExtenstions
{
    public static void AddIfNotNullOrWhitespace<T>(this List<T> list, params T[] elements)
    {
        foreach (var item in elements)
        {
            if (!string.IsNullOrWhiteSpace(item?.ToString()))
            {
                list.Add(item);
            }
        }
    }
}