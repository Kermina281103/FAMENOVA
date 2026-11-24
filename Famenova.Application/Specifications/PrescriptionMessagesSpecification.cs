using famenova.Domain.Entities;
using Famenova.Shared.Specifications;

namespace Famenova.Application.Specifications
{
    /// <summary>The whole conversation of one prescription, oldest first</summary>
    public class PrescriptionMessagesSpecification : BaseSpecification<PrescriptionMessage>
    {
        public PrescriptionMessagesSpecification(int prescriptionOrderId)
        {
            Criteria = m => m.PrescriptionOrderId == prescriptionOrderId;
            AddOrderBy(m => m.SentAtUtc);
        }
    }
}
