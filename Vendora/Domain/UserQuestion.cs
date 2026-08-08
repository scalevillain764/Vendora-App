using Domain.Products;
using Domain.Stores;
using Domain.Users;
namespace Domain.UserQuestions
{
    public class UserQuestion
    {
        public Ulid Id { get; set; }

        public Ulid UserId { get; set; }
        public User user { get; set; } = null!;

        public Ulid ProductId { get; set; }
        public Product product { get; set; } = null!;

        public Ulid StoreId { get; set; }
        public Store store { get; set; } = null!;

        public string QuestionText { get; set; }
        public string? SellerReply { get; set; }
        public List<string>? PhotoUrls { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserQuestion(Ulid userId, Ulid productId, Ulid storeId, string questionText, List<string>? photoUrls)
        {
            Id = Ulid.NewUlid();
            UserId = userId;
            ProductId = productId;
            StoreId = storeId;
            QuestionText = questionText;
            PhotoUrls = photoUrls;
            CreatedAt = DateTime.UtcNow;
            SellerReply = null;
            UpdatedAt = null;
        }
    }
}