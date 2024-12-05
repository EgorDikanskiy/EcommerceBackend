namespace Ecommerce.Models
{
    public class Category: BaseEntity
    {
        public Category(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public int Id { get; init; }
        public string Name { get; init; }
        public string Image { get; init; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
