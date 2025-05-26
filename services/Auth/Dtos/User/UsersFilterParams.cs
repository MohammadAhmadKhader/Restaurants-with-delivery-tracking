using Shared.Interfaces;

namespace Auth.Dtos.User;

public record UsersFilterParams : IPagination
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public string SortField { get; init; } = "createdAt";
    public string SortDir { get; init; } = "asc";
}