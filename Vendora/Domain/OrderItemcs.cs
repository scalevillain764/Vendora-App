using Domain.Orders;
using Domain.CartItems;
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

        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public OrderItem(Order order, CartItem item)
        {
            Id = Ulid.NewUlid();
            OrderId = order.Id;
            ProductId = item.ProductId;
            ProductName = item.Product.Name;
            Price = item.Product.Price;
            Quantity = item.Quantity;
        }
    }
}