using Microsoft.EntityFrameworkCore;
using Ecommerce.Models;


namespace Ecommerce.DataAccess
{
    public class EcommerceDbContext: DbContext
    {
        private readonly IConfiguration _configuration;

        public EcommerceDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь "Продукт принадлежит категории"
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Удаление продуктов при удалении категории
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));
        }
    }
}
