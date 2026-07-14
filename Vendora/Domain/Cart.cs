using Domain.CartItems;
using Domain.Products;
using Domain.Users;
namespace Domain.Carts
{
    public class Cart
    {
        public Ulid Id { get; set; }
        public Ulid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = [];
        public Cart(Ulid userId)
        {
            Id = Ulid.NewUlid();
            UserId = userId;
        }
    }
}