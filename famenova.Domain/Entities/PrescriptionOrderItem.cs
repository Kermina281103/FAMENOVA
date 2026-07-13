using famenova.Domain.Common;

namespace famenova.Domain.Entities
{
    public class PrescriptionOrderItem:BaseEntity<int>
    {
        public int Quantity { get; set; }
        public decimal Price  { get; set; }

        //Relationsships 
        public int PrescriptionOrderId { get; set; }
        public PrescriptionOrder PrescriptionOrder { get; set; } = default!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}