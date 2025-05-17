namespace EnterpriseAppLINQ.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }  // Relación con Order
        public Product Product { get; set; }  // Relación con Product
    }
}