using Ecommerce.Contracts;
using Ecommerce.DataAccess;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;

        public CategoriesController(EcommerceDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
        {
            var category = new Category(request.Name, request.Image);

            await _dbContext.Categories.AddAsync(category, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            var categoriesQuery = _dbContext.Categories;
            var categoriesDtos = await categoriesQuery.Select(c => new CategoryDto(c.Id, c.Name, c.Image, c.CreatedAt, c.UpdatedAt)).ToListAsync(cancellationToken: ct);

            return Ok(new GetCategoriesResponse(categoriesDtos));
        }

        // Удаление категории
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            // Найти категорию по id
            var category = await _dbContext.Categories
                .Include(c => c.Products) // Загрузка связанных продуктов
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            // Проверка: существует ли категория
            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            // Проверка: есть ли связанные продукты
            if (category.Products.Any())
            {
                return BadRequest(new { Message = "Cannot delete category with associated products" });
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(ct);

            return Ok(new { Message = "Category deleted successfully" });
        }
    }
}
