using Domain.CartItems;
using Domain.Carts;
using Domain.Products;
using Domain.Stores;
using Domain.Users;
using Domain.OrderItems;
using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Xml;
namespace Infrastructure.AppDbContexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>() // user with shop
                .HasOne(u => u.Store)
                .WithOne(s => s.Seller)
                .HasForeignKey<Store>(s => s.SellerId);

            modelBuilder.Entity<User>() // user with cart
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<Cart>() // cart with cart items
                 .HasMany(c => c.Items)
                 .WithOne(ct => ct.Cart)
                 .HasForeignKey(ct => ct.CartId);

            modelBuilder.Entity<Store>() // store with products
                .HasMany(s => s.Products)
                .WithOne(p => p.Store)
                .HasForeignKey(p => p.StoreId);

            modelBuilder.Entity<CartItem>() // cartItems with product
                .HasOne(ct => ct.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ct => ct.ProductId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Article)
                .HasIdentityOptions(startValue: 10000000);

            modelBuilder.Entity<OrderItem>() // orderItems with orders
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>() // orderItems with product
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);
        }
    }
}