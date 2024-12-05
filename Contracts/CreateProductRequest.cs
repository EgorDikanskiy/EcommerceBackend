namespace Ecommerce.Contracts
{
    public record CreateProductRequest(string Title, string Description, int Price, List<string> Images, int CategoryId);
}
