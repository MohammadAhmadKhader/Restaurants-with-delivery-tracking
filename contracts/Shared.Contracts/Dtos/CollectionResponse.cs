namespace Shared.Contracts.Dtos;

public class CollectionResponse<TModel>
    where TModel: class
{
    public List<TModel> Items { get; set; } = [];
    public int Page { get; set; }
    public int Size { get; set; }
    public int Count { get; set; }
}