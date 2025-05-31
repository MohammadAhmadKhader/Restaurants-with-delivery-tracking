using Shared.Interfaces;
namespace Shared.Utils;

public class PaginationUtils
{
    // size
    public const int DefaultSize = 10;
    public const int MinSizeAllowed = 4;
    public const int MaxSizeAllowed = 100;
    // page
    public const int DefaultPage = 1;
    public const int MinPageAllowed = 1;
    public const int MaxPageAllowed = int.MaxValue;

    public static void Normalize(IPagination filterParams)
    {
        if (filterParams.Page == null || filterParams.Page < MinPageAllowed)
        {
            filterParams.Page = DefaultPage;
        }

        if (filterParams.Size == null || filterParams.Size < MinSizeAllowed)
        {
            filterParams.Size = DefaultSize;
        }
        else if (filterParams.Size > MaxSizeAllowed)
        {
            filterParams.Size = MaxSizeAllowed;
        }
    }

    public static object ResultOf<TData>(IList<TData> data, int count, int? page, int? size)
    {
        return new { items = data, count, page, size };
    }
}