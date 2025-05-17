using EnterpriseAppLINQ.Models;
using Microsoft.AspNetCore.Mvc;
using EnterpriseAppLINQ.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnterpriseAppLINQ.Data;
using System;

namespace EnterpriseAppLINQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los pedidos realizados después de una fecha específica
        [HttpGet("search/after/{date}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAfterDate(DateTime date)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDate > date)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound($"No se encontraron pedidos después de la fecha '{date.ToShortDateString()}'");
            }

            return orders;
        }

        // Método para obtener los detalles de los productos en una orden específica
        [HttpGet("details/{orderId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrderDetails(int orderId)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .Select(od => new 
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity
                })
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound($"No se encontraron detalles para la orden con OrderId {orderId}");
            }

            return orderDetails;
        }

        // Método para obtener la cantidad total de productos de una orden
        [HttpGet("totalQuantity/{orderId}")]
        public async Task<ActionResult<int>> GetTotalProductQuantityByOrder(int orderId)
        {
            var totalQuantity = await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .SumAsync(od => od.Quantity);

            if (totalQuantity == 0)
            {
                return NotFound($"No se encontraron productos para la orden con OrderId {orderId}");
            }

            return totalQuantity;
        }

        // Método para obtener todos los pedidos y sus detalles (nombre del producto y cantidad)
        [HttpGet("allDetails")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllOrdersAndDetails()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Product)
                .Select(od => new 
                {
                    OrderId = od.OrderId,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity
                })
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound("No se encontraron detalles de pedidos.");
            }

            return orderDetails;
        }

        // Método para obtener todos los productos vendidos a un cliente específico
        [HttpGet("productsByClient/{clientId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsSoldToClient(int clientId)
        {
            var productsSold = await _context.Orders
                .Where(o => o.ClientId == clientId)  // Filtrar por ClientId
                .SelectMany(o => o.OrderDetails)  // Obtener los detalles de la orden
                .Select(od => new 
                {
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity
                })
                .ToListAsync();  // Convertir el resultado a una lista

            if (productsSold == null || !productsSold.Any())
            {
                return NotFound($"No se encontraron productos vendidos al cliente con ClientId {clientId}");
            }

            return productsSold;
        }
        
        [HttpGet("{orderId}/details-with-products")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderWithProducts(int orderId)
        {
            var dto = await _context.Orders
                .AsNoTracking()  // opcional: solo lectura
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.OrderId == orderId)
                .Select(o => new OrderDetailsDto
                {
                    OrderId   = o.OrderId,
                    OrderDate = o.OrderDate,
                    Products  = o.OrderDetails
                        .Select(od => new ProductDto
                        {
                            ProductName = od.Product.Name,
                            Quantity    = od.Quantity,
                            Price       = od.Product.Price
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound($"No se encontró la orden {orderId}.");

            return Ok(dto);
        }
    }
}
