using _user;
using _product;
using _cart;
namespace _cartItem
{
    public class CartItem
    {
        public Ulid Id { get; set; }

        // references
        public Ulid CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public Ulid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public CartItem (Ulid cartId, Ulid productId, int quantity)
        {
            Id = Ulid.NewUlid();
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}