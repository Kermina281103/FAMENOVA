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
   public class OrderItemBatchConfiguration:IEntityTypeConfiguration<OrderItemBatch>
    {
        public void Configure(EntityTypeBuilder<OrderItemBatch> b)
        {
            b.HasOne(x => x.OrderItem).WithMany(i => i.Allocations)
             .HasForeignKey(x => x.OrderItemId).OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.MedicineBatch).WithMany()
             .HasForeignKey(x => x.MedicineBatchId).OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.OrderItemId, x.MedicineBatchId }).IsUnique();
        }
    }
}
