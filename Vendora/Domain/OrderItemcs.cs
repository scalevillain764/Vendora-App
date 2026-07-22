using Domain.Orders;
using Domain.CartItems;
using Domain.Users;
using Domain.Stores;
using Domain.Products;
namespace Domain.OrderItems
{
    public class OrderItem
    {
        public Ulid Id { get; set; }

        public Ulid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Ulid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Ulid SellerId { get; set; }

        public Ulid StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public string ProductName { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }

        public OrderItem(Ulid orderId, CartItem item, Ulid sellerId)
        {
            Id = Ulid.NewUlid();
            SellerId = sellerId;
            OrderId = orderId;
            ProductId = item.ProductId;
            ProductName = item.Product.Name;
            PricePerUnit = item.PricePerUnit;
            Quantity = item.Quantity;
        }
    }
}