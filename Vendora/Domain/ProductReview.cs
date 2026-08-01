using Domain.Users;
using Domain.Products;
using Domain.Stores;
namespace Domain.ProductReviews
{
    public class ProductReview
    {
        public Ulid Id { get; set; }

        public Ulid UserId { get; set; }
        public User user { get; set; } = null!;

        public Ulid ProductId { get; set; }
        public Product product { get; set; } = null!;

        public Ulid StoreId { get; set; }
        public Store store { get; set; } = null!;

        public string? ReviewText { get; set; }
        public string? SellerReply { get; set; }
        public int Rating { get; set; }
        public List<string>? PhotoUrls { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ProductReview(Ulid userId, Ulid productId, Ulid storeId, string? reviewText, 
            int rating, List<string>? photoUrls)
        {
            Id = Ulid.NewUlid();
            UserId = userId;
            ProductId = productId;
            StoreId = storeId;
            ReviewText = reviewText;
            Rating = rating;
            PhotoUrls = photoUrls;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
            SellerReply = null;
        }
    }
}