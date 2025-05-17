using EnterpriseAppLINQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnterpriseAppLINQ.Data;

namespace EnterpriseAppLINQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los productos
        [HttpGet("search/price/{value}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPrice(decimal value)
        {
            // 1. EF Core ejecuta la consulta y devuelve List<Product>
            var products = await _context.Products
                .Where(p => p.Price > value)
                .ToListAsync();

            // 2. Si no hay ningún producto, devolvemos 404
            if (products == null || !products.Any())
            {
                return NotFound($"No se encontraron productos con un precio mayor a {value}.");
            }

            // 3. De lo contrario devolvemos la lista completa de Product
            return products;
        }



        // Método para obtener el producto más caro
        [HttpGet("mostExpensive")]
        public async Task<ActionResult<Product>> GetMostExpensiveProduct()
        {
            // EF va a devolver un Product completamente poblado
            var product = await _context.Products
                .OrderByDescending(p => p.Price)    // ordenar de mayor a menor
                .FirstOrDefaultAsync();             // tomar sólo el primero

            if (product == null)
                return NotFound("No se encontraron productos.");

            return product;
        }

        // Método para obtener el promedio de precios de los productos
        [HttpGet("averagePrice")]
        public async Task<ActionResult<decimal>> GetAveragePrice()
        {
            var averagePrice = await _context.Products
                .AverageAsync(p => p.Price);

            if (averagePrice == 0)
            {
                return NotFound("No se encontraron productos.");
            }

            return Ok(averagePrice);
        }

        // Método para obtener todos los productos que no tienen descripción
        [HttpGet("noDescription")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsNoDescription()
        {
            // Usamos LINQ para filtrar productos donde la descripción es nula o está vacía
            var products = await _context.Products
                .Where(p => string.IsNullOrEmpty(p.Description))  // Filtrar por productos sin descripción
                .ToListAsync();  // Convertir el resultado a una lista

            if (products == null || !products.Any())
            {
                return NotFound("No se encontraron productos sin descripción.");
            }

            return products;
        }
    }
}
