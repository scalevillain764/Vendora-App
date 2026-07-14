using Domain.Stores;
using Domain.CartItems;
using Domain.Users;
namespace Domain.Products
{
    public class Product
    {
        public Ulid Id { get; set; }

        // references
        public Ulid StoreId { get; private set; }

        public Store Store = null!;
        public ICollection<CartItem> Items { get; private set; };

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public long Article { get; set; }
        
        // preview
        public string? PreviewUrl { get; set; }
        public bool IsDeleted { get; set; }
        public Product(Ulid storeId, int categoryId, 
            string name, string? description, 
            decimal price, int quantity,
            string? previewUrl)
        {
            Id = Ulid.NewUlid();
            StoreId = storeId;
            CategoryId = categoryId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Article = 0;
            PreviewUrl = previewUrl;
            IsDeleted = false;
        }
    }
}