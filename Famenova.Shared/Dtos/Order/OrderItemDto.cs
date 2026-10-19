namespace Famenova.Shared.Dtos.Order
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
    }
}