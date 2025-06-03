namespace Shared.Dtos;

public record PagedResult<TData>(List<TData> Items, int? Page, int? Size, int Count);