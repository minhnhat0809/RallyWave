namespace MatchManagement.Ultility;

public class ListExtensions
{
    public List<T>? Paging<T>(List<T>? items, int pageNumber, int pageSize)
    {
        if (items == null || items.Count == 0)
        {
            return items;
        }

        return items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<T>? Sort<T, TKey>(
        List<T>? items,
        Func<T, TKey> keySelector,
        bool ascending)
    {
        if (items == null || items.Count == 0)
        {
            return items;
        }

        return ascending ? items.OrderBy(keySelector).ToList() : items.OrderByDescending(keySelector).ToList();
    }
}