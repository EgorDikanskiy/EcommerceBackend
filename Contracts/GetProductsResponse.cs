namespace Ecommerce.Contracts
{
    public record GetProductsResponse(List<ProductDto> products);
    public record ProductDto(int Id, string Title, int Price, string Description, List<string> Images, DateTime CreatedAt, DateTime UpdatedAt, CategoryDto Category);
}
