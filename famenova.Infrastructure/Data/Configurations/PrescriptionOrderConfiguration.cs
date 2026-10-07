using famenova.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Data.Configurations
{
    public class PrescriptionOrderConfiguration : IEntityTypeConfiguration<PrescriptionOrder>
    {
        public void Configure(EntityTypeBuilder<PrescriptionOrder> builder)
        {
            
            builder.OwnsOne(p => p.DeliveryAddress);

            builder.Property(p => p.TotalPrice).HasPrecision(18, 2);

            builder.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(p => p.PrescriptionImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(p => p.ClarificationImageUrl).HasMaxLength(500);
            builder.Property(p => p.RejectReason).HasMaxLength(1000);
            builder.Property(p => p.ClarificationRequest).HasMaxLength(1000);
            builder.Property(p => p.ClarificationResponse).HasMaxLength(1000);

            // Don't remove prescription if customer deleted
            builder.HasOne(p => p.Customer)
                   .WithMany()
                   .HasForeignKey(p => p.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Items)
                   .WithOne(i => i.PrescriptionOrder)
                   .HasForeignKey(i => i.PrescriptionOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            
            builder.HasOne(p => p.Order)
                   .WithOne()
                   .HasForeignKey<PrescriptionOrder>(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.OrderId)
                   .IsUnique()
                   .HasFilter("[OrderId] IS NOT NULL");

            
            builder.HasIndex(p => new { p.Status, p.CreatedOn });
            builder.HasIndex(p => p.CustomerId);
        }
    }
}
