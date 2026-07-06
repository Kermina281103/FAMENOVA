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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.HasOne(x => x.User)
                .WithMany(y => y.Orders)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(x => x.City).HasMaxLength(30).IsRequired();
                address.Property(x => x.street).HasMaxLength(30).IsRequired();
                address.Property(x => x.ZipCode).HasMaxLength(30);

                } );

            builder.Property(o => o.PhoneNumber)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(o => o.OrderStatus)
                .HasConversion<string>();

            builder.Property(o => o.OrderStatus)
                .HasConversion<string>();



        }
    }
}
