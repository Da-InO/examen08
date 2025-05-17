// Models/Order.cs
namespace EnterpriseAppLINQ.Models
{
    public class Order
    {
        public int OrderId  { get; set; }

        // FK → Client
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        // ⇄ 1-n con OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public DateTime OrderDate { get; set; }
    }
}