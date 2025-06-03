using BlazorShop.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorShop.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).HasMaxLength(50);
            });

            // Начальные данные
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Ноутбук", Description = "Игровой ноутбук", Price = 75000, Quantity = 5, Category = "Электроника" },
                new Product { Id = 2, Name = "Мышь", Description = "Беспроводная мышь", Price = 2500, Quantity = 20, Category = "Аксессуары" },
                new Product { Id = 3, Name = "Клавиатура", Description = "Механическая клавиатура", Price = 8000, Quantity = 10, Category = "Аксессуары" }
            );
        }
    }
}