using Ecommerce.Contracts;
using Ecommerce.DataAccess;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController: ControllerBase
    {
        private readonly EcommerceDbContext _dbContext;

        public ProductsController(EcommerceDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
        {
            var product = new Product(request.Title, request.Description, request.Price, request.Images, request.CategoryId);

            await _dbContext.Products.AddAsync(product, ct);
            await _dbContext.SaveChangesAsync(ct);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetProductsRequest request, CancellationToken ct)
        {
            var productsQuery = _dbContext.Products
            .Include(p => p.Category) // Загрузка связанной категории
            .Where(p =>
                (string.IsNullOrWhiteSpace(request.title) ||
                 p.Title.ToLower().Contains(request.title.ToLower())) &&
                (!request.CategoryId.HasValue || p.Category.Id == request.CategoryId) &&
                (!request.price_min.HasValue || p.Price >= request.price_min) && // Фильтрация по минимальной цене
                (!request.price_max.HasValue || p.Price <= request.price_max)    // Фильтрация по максимальной цене
            );

            Expression<Func<Product, object>> selectorKey = request.SortItem?.ToLower() switch
            {
                "date" => product => product.CreatedAt,
                "title" => product => product.Title,
                _ => product => product.Id
            };

            productsQuery = request.SortOrder == "desc"
                ? productsQuery.OrderByDescending(selectorKey)
                : productsQuery.OrderBy(selectorKey);

            productsQuery = productsQuery.Skip((int)request.offset).Take((int)request.limit);

            var productDtos = await productsQuery
                .Select(p => new ProductDto(
                    p.Id,
                    p.Title,
                    p.Price,
                    p.Description,
                    p.Images,
                    p.CreatedAt,
                    p.UpdatedAt,
                    new CategoryDto(
                        p.Category.Id,
                        p.Category.Name,
                        p.Category.Image,
                        p.Category.CreatedAt,
                        p.Category.UpdatedAt
                    )
                ))
                .ToListAsync(cancellationToken: ct);

            return Ok(new GetProductsResponse(productDtos));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken ct)
        {
            // Ищем товар по id
            var product = await _dbContext.Products
                .Include(p => p.Category) // Если хотите загрузить категорию товара
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken: ct);

            // Если товар не найден, возвращаем ошибку 404
            if (product == null)
            {
                return NotFound(new { Message = "Product not found" });
            }

            // Преобразуем товар в DTO
            var productDto = new ProductDto(
                product.Id,
                product.Title,
                product.Price,
                product.Description,
                product.Images,
                product.CreatedAt,
                product.UpdatedAt,
                new CategoryDto(
                        product.Category.Id,
                        product.Category.Name,
                        product.Category.Image,
                        product.Category.CreatedAt,
                        product.Category.UpdatedAt
                    )
            );

            return Ok(productDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            // Найти товар по id
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            // Проверка: существует ли товар
            if (product == null)
            {
                return NotFound(new { Message = "Product not found" });
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(ct);

            return Ok(new { Message = "Product deleted successfully" });
        }
    }
}
