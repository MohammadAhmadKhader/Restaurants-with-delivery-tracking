using Shared.Contracts.Interfaces;
namespace Shared.Utils;

public class SortingUtils(List<string> sortFields, string defaultSortField = "createdAt", string defaultSortDir = "desc")
{
    private readonly List<string> _sortFields = sortFields;
    private readonly string _defaultSortField = defaultSortField;
    private readonly string _defaultSortDir = defaultSortDir;

    public void Normalize(ISort sortable)
    {
        if (sortable.SortDir == null || sortable.SortDir != "asc" || sortable.SortDir != "desc")
        {
            sortable.SortDir = _defaultSortDir;
        }

        if (sortable.SortField == null || !_sortFields.Contains(sortable.SortField))
        {
            sortable.SortField = _defaultSortField;
        }
    }
}