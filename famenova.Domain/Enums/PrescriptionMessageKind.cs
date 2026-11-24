namespace famenova.Domain.Enums
{
    public enum PrescriptionMessageKind
    {
        /// <summary>Free message (either side)</summary>
        Message = 0,
        /// <summary>The pharmacist asked a question: the prescription moved to NeedsClarification</summary>
        ClarificationRequest = 1,
        /// <summary>The customer's answer while the prescription was in NeedsClarification</summary>
        ClarificationReply = 2
    }
}
