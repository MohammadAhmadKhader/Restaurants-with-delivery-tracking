using Shared.Contracts.Interfaces;

namespace Auth.Contracts.Dtos.User;

public record UsersFilterParams : IPagination, ISort
{
    public int? Page { get; set; }
    public int? Size { get; set; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public string? SortField { get; set; }
    public string? SortDir { get; set; }
}