using famenova.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace famenova.Infrastructure.Data.Configurations
{
    public class PrescriptionMessageConfiguration : IEntityTypeConfiguration<PrescriptionMessage>
    {
        public void Configure(EntityTypeBuilder<PrescriptionMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Body).HasMaxLength(PrescriptionMessage.MaxBodyLength);
            builder.Property(m => m.AttachmentUrl).HasMaxLength(500);

            builder.HasOne(m => m.PrescriptionOrder)
                   .WithMany(p => p.Messages)
                   .HasForeignKey(m => m.PrescriptionOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // the thread, in order
            builder.HasIndex(m => new { m.PrescriptionOrderId, m.SentAtUtc });
            // unread counters
            builder.HasIndex(m => new { m.Sender, m.ReadAtUtc });
        }
    }
}
