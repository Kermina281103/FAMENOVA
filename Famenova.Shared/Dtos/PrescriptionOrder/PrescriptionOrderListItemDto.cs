namespace Famenova.Shared.Dtos.PrescriptionOrder
{
    public class PrescriptionOrderListItemDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public  decimal? TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PrescriptionImageUrl { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? ClarificationRespondedAt { get; set; }
    }
}