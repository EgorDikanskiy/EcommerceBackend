namespace Ecommerce.Contracts
{
    public record GetProductsRequest(
        string? title, 
        string? SortItem, 
        string? SortOrder, 
        int? CategoryId, 
        int? price_min,
        int? price_max,
        int? offset = 0,
        int? limit = 1000000000
        );
}
