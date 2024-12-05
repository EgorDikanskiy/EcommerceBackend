namespace Ecommerce.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }

        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
