using System.Collections.Generic;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of seeding users into the database
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "adminpassword", Role = "Admin" },
                new User { Id = 2, Username = "user", Password = "userpassword", Role = "User" }
            );
        }
    }
}
