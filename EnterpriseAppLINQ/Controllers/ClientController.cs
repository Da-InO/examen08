using EnterpriseAppLINQ.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EnterpriseAppLINQ.Data;

namespace EnterpriseAppLINQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // Método para obtener un cliente por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // Método para obtener clientes cuyo nombre contiene un valor específico
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsByName(string name)
        {
            var clients = await _context.Clients
                .Where(c => c.Name.Contains(name))
                .ToListAsync();

            if (clients == null || !clients.Any())
            {
                return NotFound($"No se encontraron clientes con el nombre que contiene '{name}'");
            }

            return clients;
        }

        // Método para obtener el cliente con mayor número de pedidos
        [HttpGet("mostOrders")]
        public async Task<ActionResult<Client>> GetClientWithMostOrders()
        {
            // Usamos LINQ para agrupar los pedidos por ClientId y contar el número de pedidos
            var clientWithMostOrders = await _context.Orders
                .GroupBy(o => o.ClientId)  // Agrupar por ClientId
                .Select(g => new
                {
                    ClientId = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(g => g.OrderCount)  // Ordenar por el número de pedidos de forma descendente
                .FirstOrDefaultAsync();  // Obtener el primer grupo (el que tiene más pedidos)

            if (clientWithMostOrders == null)
            {
                return NotFound("No se encontraron clientes.");
            }

            // Buscar el cliente correspondiente
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientWithMostOrders.ClientId);

            if (client == null)
            {
                return NotFound("Cliente no encontrado.");
            }

            return client;  // Retorna el cliente con más pedidos
        }

        // Método para actualizar un cliente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.ClientId)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        // ClientController.cs
        [HttpGet("purchasedProduct/{productId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetClientsWhoPurchasedProduct(int productId)
        {
            var clients = await _context.OrderDetails
                .Where(od => od.ProductId == productId)    // 1) Filtrar por ProductId
                .Select(od => new 
                {
                    ClientName = od.Order.Client.Name      // 2) Proyectar el nombre del cliente
                })
                .Distinct()                                 // 3) Quitar duplicados
                .ToListAsync();                             // 4) Materializar la lista

            if (!clients.Any())
                return NotFound($"No se encontraron clientes que hayan comprado el producto con ProductId {productId}");

            return Ok(clients);
        }


        // Método para crear un cliente
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.ClientId }, client);
        }

        // Método para eliminar un cliente
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}
