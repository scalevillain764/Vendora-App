using _product;
using _user;
namespace _store
{
    public class Store
    {
        public Ulid Id { get; set; }

        // references
        public Ulid SellerId { get; private set; }
        public User Seller { get; set; } = null!; // np

        public string Name { get; set; }
        public string? Description { get; set; }
        public string? UrlAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Product> Products { get; set; } = new(); // np

        public Store(Ulid sellerId, string name, string? description, string? urlAvatar)
        {
            SellerId = sellerId;
            Name = name;
            Description = description;
            UrlAvatar = urlAvatar;
            CreatedAt = DateTime.UtcNow;
        }
    }
}