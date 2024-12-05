namespace Ecommerce.Models
{
    public class Product: BaseEntity
    {   
        public Product(string title, string description, int price, List<string> images, int categoryId) 
        {
            Title = title;
            Description = description;
            Price = price;
            Images = images;
            CategoryId = categoryId;
        }

        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int Price { get; init; }
        public List<string> Images { get; init; }

        // Связь с категорией
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
