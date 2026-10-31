using famenova.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(x => x.Amount).HasPrecision(18, 2);

            builder.Property(p => p.MerchantRefNumber).HasMaxLength(64).IsRequired();
            builder.HasIndex(p => p.MerchantRefNumber).IsUnique();


            builder.Property(p => p.ProvierderReference).HasMaxLength(100);
            builder.Property(p => p.FailureReason).HasMaxLength(500);
            builder.Property(p => p.RefundReason).HasMaxLength(500);

            builder.HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.RequiredRefund);

        }
    }
}
