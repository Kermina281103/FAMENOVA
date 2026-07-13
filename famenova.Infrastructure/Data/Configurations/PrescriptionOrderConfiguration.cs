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
            builder.HasKey(k => k.Id);
            builder.Property(po => po.TotalPrice)
               .HasColumnType("decimal(18,2)");

            builder.HasOne(po => po.Customer)
                .WithMany()
                .HasForeignKey(po => po.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.DeliveryAddress, address =>
            {
                address.Property(x => x.City).HasMaxLength(30).IsRequired();
                address.Property(x => x.street).HasMaxLength(30).IsRequired();
                address.Property(x => x.ZipCode).HasMaxLength(30);

            });

            builder.Property(ps => ps.Status)
                .HasConversion<string>();
        }
    }
}
