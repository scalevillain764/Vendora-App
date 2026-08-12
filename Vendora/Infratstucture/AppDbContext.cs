using Domain.CartItems;
using Domain.Carts;
using Domain.Favourites;
using Domain.OrderItems;
using Domain.Orders;
using Domain.ProductReviews;
using Domain.UserQuestions;
using Domain.Products;
using Domain.Stores;
using Domain.Transactions;
using Domain.Users;
using Infrastructure.UlidToStringConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Domain.ProductStatisticsForStores;
using System.Xml;
namespace Infrastructure.AppDbContexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet <OrderItem> OrderItems { get; set; }
        public DbSet <Cart> Carts { get; set; }
        public DbSet <CartItem> CartItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }      
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<UserQuestion> UserQuestions { get; set; }
        public DbSet<ProductStatistics> ProductStatistics { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<Ulid>()
                .HaveConversion<UlidToStringConverter>()
                .HaveMaxLength(26)
                .AreFixedLength();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // query filters
            modelBuilder.Entity<User>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Store>()
                .HasQueryFilter(x => !x.IsDeleted);

            // user
            modelBuilder.Entity<User>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<User>() // user with store
                .HasOne(u => u.Store)
                .WithOne(s => s.Seller)
                .HasForeignKey<Store>(s => s.SellerId);

            modelBuilder.Entity<User>() // user with cart
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<User>() // user with orders
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            // cart
            modelBuilder.Entity<Cart>()
                .HasKey(x => x.UserId);

            modelBuilder.Entity<Cart>() // cart with cart items
                 .HasMany(c => c.Items)
                 .WithOne(ct => ct.Cart)
                 .HasForeignKey(ct => ct.CartId);

            modelBuilder.Entity<CartItem>() // cartItems with product
                .HasOne(ct => ct.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ct => ct.ProductId);

            // product
            modelBuilder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);

            // store
            modelBuilder.Entity<Store>()
                .HasQueryFilter(x => !x.IsDeleted);

            modelBuilder.Entity<Store>() // store with products
                .HasMany(s => s.Products)
                .WithOne(p => p.Store)
                .HasForeignKey(p => p.StoreId);

            // order
            modelBuilder.Entity<OrderItem>() // orderItems with orders
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>() // orderItems with products
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Article)
                .HasIdentityOptions(startValue: 10000000);

            // payment
            modelBuilder.Entity<Transaction>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Transactions)
                .HasForeignKey(p => p.OrderId);

            // favourite
            modelBuilder.Entity<Favourite>()
                .HasKey(x => new { x.UserId, x.ProductId });

            // product preview
            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.product)
                .WithMany(p => p.ProductReviews)
                .HasForeignKey(pr => pr.ProductId);

            modelBuilder.Entity<ProductReview>()
                .HasOne(pr => pr.user)
                .WithMany(u => u.Reviews)
                .HasForeignKey(pr => pr.UserId);

            // user question
            modelBuilder.Entity<UserQuestion>()
                .HasOne(uq => uq.product)
                .WithMany(p => p.UserQuestions)
                .HasForeignKey(uq => uq.ProductId);

            // product statistics
            modelBuilder.Entity<ProductStatistics>()
                .HasKey(x => x.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Statistics)
                .WithOne(s => s.Product)
                .HasForeignKey<ProductStatistics>(s => s.ProductId);

            // remove auto-increment
            foreach (var dbSet in modelBuilder.Model.GetEntityTypes())
            {
                var primary_key = dbSet.FindPrimaryKey();
                if(primary_key != null)
                {
                    foreach(var property in primary_key.Properties)
                    {
                        if(property.ClrType == typeof(Ulid))
                            property.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
                    }
                }
            }
        }

        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

                string? connectionString = null;

                if (args.Length > 0)
                    connectionString = args[0]; 
                else
                {
                    string? connectionPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
                    string? connectionDatabase = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
                    string? connectionUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
                    string? connectionPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

                    if (string.IsNullOrEmpty(connectionPort) ||
                        string.IsNullOrEmpty(connectionDatabase) ||
                        string.IsNullOrEmpty(connectionUsername) ||
                        string.IsNullOrEmpty(connectionPassword))
                        throw new InvalidOperationException(
                            $"Configuration error in PostgreSQL:\n" +
                            $"PORT: '{connectionPort ?? "Undefined"}'\n" +
                            $"DATABASE: '{connectionDatabase ?? "Undefined"}'\n" +
                            $"USERNAME: '{connectionUsername ?? "Undefined"}'\n" +
                            $"PASSWORD: '{connectionPassword ?? "Undefined"}'"
                        );

                    connectionString =
                        $"Host=localhost;Port={connectionPort};Database=VendoraAppD{connectionDatabase};Username={connectionUsername};Password={connectionPassword}";
                }
  
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException(
                        "Строка подключения не найдена ни в переменной окружения, ни в аргументах команды. " +
                        "Передайте её так: dotnet ef database update -- \"Host=localhost;...\"");
                }

                optionsBuilder.UseNpgsql(connectionString);
                return new AppDbContext(optionsBuilder.Options);
            }
        }
    }
}