namespace Shared.Contracts.Interfaces;

public interface ISort
{
    string? SortField { get; set; }
    string? SortDir { get; set; }
}