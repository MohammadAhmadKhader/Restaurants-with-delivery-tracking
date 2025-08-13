namespace Shared.Extensions;
public static class CollectionsExtensions
{
    /// <summary>
    /// Custom AddRange
    /// </summary>
    public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            target.Add(item);
        }
    }

    public static void AddRangeIf<T>(this ICollection<T> target, IEnumerable<T> items, Func<T, bool> conditionFunc)
    {
        foreach (var item in items)
        {
            if (conditionFunc(item))
            {
                target.Add(item);
            }
        }
    }

    /// <summary>
    /// Custom AddRange
    /// </summary>
    public static void AddRange<T>(this List<T> target, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            target.Add(item);
        }
    }
}