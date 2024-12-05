namespace Ecommerce.Contracts
{
    public record GetCategoriesResponse(List<CategoryDto> categories);
    public record CategoryDto(int Id, string Name, string Image, DateTime CreatedAt, DateTime UpdatedAt);

}
