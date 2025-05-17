namespace EnterpriseAppLINQ.DTOs
{
    public class ClientOrderDto
    {
        public string            ClientName { get; set; } = null!;
        public List<OrderDto>    Orders     { get; set; } = new();
    }
}