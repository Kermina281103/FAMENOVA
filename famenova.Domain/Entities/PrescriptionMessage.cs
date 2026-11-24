using famenova.Domain.Common;
using famenova.Domain.Enums;
using famenova.Domain.Exceptions;

namespace famenova.Domain.Entities
{
    /// <summary>One message of the conversation of a prescription order</summary>
    public class PrescriptionMessage : BaseEntity<int>
    {
        public const int MaxBodyLength = 2000;

        public int PrescriptionOrderId { get; set; }
        public PrescriptionOrder PrescriptionOrder { get; set; } = default!;

        public MessageSender Sender { get; set; }
        /// <summary>The user who wrote it (null for messages copied from the old clarification fields)</summary>
        public int? SenderUserId { get; set; }

        public PrescriptionMessageKind Kind { get; set; }
        public string? Body { get; set; }
        /// <summary>Relative path of an image / PDF (validated by FileService)</summary>
        public string? AttachmentUrl { get; set; }

        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
        /// <summary>When the OTHER side read it (null = unread)</summary>
        public DateTime? ReadAtUtc { get; set; }

        public static PrescriptionMessage Create(
            int prescriptionOrderId, MessageSender sender, int? senderUserId,
            PrescriptionMessageKind kind, string? body, string? attachmentUrl)
        {
            var text = string.IsNullOrWhiteSpace(body) ? null : body.Trim();
            if (text is null && string.IsNullOrEmpty(attachmentUrl))
                throw new DomainException("A message needs text or an attachment.");
            if (text?.Length > MaxBodyLength)
                throw new DomainException($"A message can be at most {MaxBodyLength} characters.");

            return new PrescriptionMessage
            {
                PrescriptionOrderId = prescriptionOrderId,
                Sender = sender,
                SenderUserId = senderUserId,
                Kind = kind,
                Body = text,
                AttachmentUrl = attachmentUrl,
                SentAtUtc = DateTime.UtcNow
            };
        }

        public void MarkRead()
        {
            ReadAtUtc ??= DateTime.UtcNow;
        }
    }
}
