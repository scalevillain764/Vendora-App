using Domain.Orders;
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

        public Ulid StoreId { get; set; }
        public Store Store { get; set; } = null!;

        public Ulid SellerId { get; set; }

        public string ProductName { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Quantity { get; set; }
        private OrderItem() { }
        internal OrderItem(Ulid orderId, Ulid sellerId, Ulid storeId, Ulid productId, 
            string productName, 
            decimal productPrice,
            int productQuantity)
        {      
            Id = Ulid.NewUlid();
            OrderId = orderId;
            SellerId = sellerId;
            StoreId = storeId;
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            PricePerUnit = productPrice;
            Quantity = productQuantity;
        }
    }
}