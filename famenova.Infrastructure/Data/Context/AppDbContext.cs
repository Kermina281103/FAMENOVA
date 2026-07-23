using famenova.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,
    IdentityRole<int>, 
    int,               
    IdentityUserClaim<int>,
    IdentityUserRole<int>,
    IdentityUserLogin<int>,
    IdentityRoleClaim<int>,
    IdentityUserToken<int>>
    {
        #region DBSets 
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Medicine> Medicine { get; set; }
        public DbSet<MedicineBatch> Batches { get; set; }
        public DbSet<PrescriptionOrder> PrescriptionOrder { get; set; }
        public DbSet<PrescriptionOrderItem> PrescriptionOrderItem { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Entity<IdentityRole<int>>().HasData(
           new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
               new IdentityRole<int> { Id=2,Name="Customer",NormalizedName="CUSTOMER"}
            );
            #region AdminUser
            var AdminUserId = 1;

            var AdminUser = new ApplicationUser
            {
                Id = AdminUserId,
                UserName = "Kermina Maged",
                NormalizedUserName = "KERMINA MAGED",
                Email = "carmina.maged.matta@gmail.com",
                NormalizedEmail = "CARMINA.MAGED.MATTA@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumber = "01229548237",
                FirstName = "Kermina",
                LastName = "Maged",
                SecurityStamp = "B379A222-38B6-4A59-A92B-3C986EE27F47",
                ConcurrencyStamp= "C822B111-22B2-4A59-A92B-3C986EE27F48",
                PasswordHash = "AQAAAAIAAYagAAAAEIe8ZlKz9C/2iIq6E8uL9v27P9XgTux5mX9o8vN9w4X7r+LwW3JzE6n2yM/1z7bXvw=="
            };
           

            
            builder.Entity<ApplicationUser>().HasData(AdminUser);

            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int>
                {
                    UserId =AdminUserId,
                    RoleId = 1
                }
                );
            #endregion
        }

    }
}
