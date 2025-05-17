public class Product
{
    public int     ProductId   { get; set; }
    public string  Name        { get; set; } = null!;
    public string? Description { get; set; }    // ahora acepta nulls
    public decimal Price       { get; set; }
}