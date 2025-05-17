namespace EnterpriseAppLINQ.DTOs
{
    public class ProductDto
    {
        public string  ProductName { get; set; } = null!;
        public int     Quantity    { get; set; }
        public decimal Price       { get; set; }
    }
}