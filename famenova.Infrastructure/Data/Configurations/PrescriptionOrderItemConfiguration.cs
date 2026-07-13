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
    public class PrescriptionOrderItemConfiguration : IEntityTypeConfiguration<PrescriptionOrderItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionOrderItem> builder)
        {
            builder.HasKey(k => k.Id);
            builder.Property(poi => poi.Price)
               .HasColumnType("decimal(18,2)");
            builder.HasOne(p => p.PrescriptionOrder)
                .WithMany(I => I.Items)
                .HasForeignKey(PId => PId.PrescriptionOrderId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(p => p.Product)
              .WithMany()
              .HasForeignKey(p => p.ProductId)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
