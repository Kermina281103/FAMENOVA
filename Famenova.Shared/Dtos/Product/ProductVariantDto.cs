namespace Famenova.Shared.Dtos.Product
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public string? VariantLabel { get; set; }
        public decimal price { get; set; }

        public int Stock { get; set; }
    }
}