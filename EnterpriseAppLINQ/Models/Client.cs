using System.ComponentModel.DataAnnotations;

namespace EnterpriseAppLINQ.Models
{
    public class Client
    {
        public int ClientId { get; set; } // ID del cliente
        public string Name { get; set; }  // Nombre del cliente

        // Otras propiedades como email, teléfono, etc.
        public string Email { get; set; }
    }
}