using Shared.Contracts.Interfaces;

namespace Shared.Dtos;

public class PagedRequest: IPagination
{
    public int? Page { get; set; }
    
    public int? Size { get; set; }
}