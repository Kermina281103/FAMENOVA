using Microsoft.AspNetCore.Http;

namespace Famenova.Shared.Dtos.PrescriptionOrder
{
    public class PrescriptionMessageDto
    {
        public int Id { get; set; }
        public int PrescriptionOrderId { get; set; }
        /// <summary>"Customer" | "Pharmacist"</summary>
        public string Sender { get; set; } = default!;
        /// <summary>"Message" | "ClarificationRequest" | "ClarificationReply"</summary>
        public string Kind { get; set; } = default!;
        public string? Body { get; set; }
        /// <summary>Absolute URL of the image / PDF, or null</summary>
        public string? AttachmentUrl { get; set; }
        public DateTime SentAtUtc { get; set; }
        /// <summary>When the other side read it (null = not read yet)</summary>
        public DateTime? ReadAtUtc { get; set; }
    }

    /// <summary>multipart/form-data: Body and/or Attachment (JPG, PNG, WebP or PDF, max 5 MB)</summary>
    public class SendPrescriptionMessageDto
    {
        public string? Body { get; set; }
        public IFormFile? Attachment { get; set; }
    }

    public class UnreadMessagesItemDto
    {
        public int PrescriptionOrderId { get; set; }
        public int Count { get; set; }
        public DateTime LastSentAtUtc { get; set; }
        /// <summary>The newest unread message is a clarification request / reply</summary>
        public bool IsClarification { get; set; }
    }

    /// <summary>Messages written by the OTHER side that the caller has not read yet</summary>
    public class UnreadMessagesSummaryDto
    {
        public int Total { get; set; }
        public List<UnreadMessagesItemDto> Items { get; set; } = new();
    }
}
