using Domain.Users;
using Domain.Products;
using Domain.Carts;
namespace Domain.CartItems
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
        public decimal PricePerUnit { get; set; }

        public CartItem (Ulid cartId, Ulid productId, decimal pricePerUnit)
        {
            Id = Ulid.NewUlid();
            CartId = cartId;
            ProductId = productId;
            Quantity = 1;
            PricePerUnit = pricePerUnit;
        }
    }
}