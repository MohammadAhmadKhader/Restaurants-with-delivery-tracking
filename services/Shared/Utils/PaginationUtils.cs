using Shared.Contracts.Dtos;
using Shared.Contracts.Interfaces;
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

    public static void Normalize(IPagination pagination)
    {
        int? page = pagination.Page;
        int? size = pagination.Size;

        HandleNormalization(ref page, ref size);

        pagination.Page = page;
        pagination.Size = size;
    }

    public static void Normalize(ref int? page, ref int? size)
    {
        HandleNormalization(ref page, ref size);
    }
    public static object ResultOf<TData>(List<TData> data, int count, int? page, int? size)
    {
        return new PagedResult<TData>(Items: data, Page: page, Size: size, Count: count);
    }

    private static void HandleNormalization(ref int? page, ref int? size)
    {
        if (page == null || page < MinPageAllowed)
        {
            page = DefaultPage;
        }

        if (size == null || size < MinSizeAllowed)
        {
            size = DefaultSize;
        }
        else if (size > MaxSizeAllowed)
        {
            size = MaxSizeAllowed;
        }
    }
}