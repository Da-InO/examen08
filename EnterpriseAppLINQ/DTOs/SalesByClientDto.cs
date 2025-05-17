namespace EnterpriseAppLINQ.DTOs
{
    public class SalesByClientDto
    {
        public string  ClientName { get; set; } = null!;
        public decimal TotalSales { get; set; }
    }
}