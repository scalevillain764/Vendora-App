using Domain.Users;
using Domain.Products;
namespace Domain.Favourites
{
    public class Favourite
    {
        public Ulid UserId { get; set; }
        public User User { get; set; } = null!;
        public Ulid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public Favourite(Ulid userId, Ulid productId)
        {
            UserId = userId;
            ProductId = productId;
        }
    }
}