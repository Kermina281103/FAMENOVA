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
    public class MedicineBatchConfiguration : IEntityTypeConfiguration<MedicineBatch>
    {
        public void Configure(EntityTypeBuilder<MedicineBatch> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(m => m.Medicine)
                .WithMany(b => b.Batches)
                .HasForeignKey(mb => mb.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
