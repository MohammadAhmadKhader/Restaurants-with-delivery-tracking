namespace Shared.Contracts.Interfaces;

public interface IPagination
{
    int? Page { get; set; }
    int? Size { get; set; }
}