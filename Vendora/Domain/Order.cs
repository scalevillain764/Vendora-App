using Domain.OrderItems;
using Domain.Users;
namespace Domain.Orders
{
    public class Order
    {
        public Ulid Id { get; set; }

        public Ulid UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal TotalPrice { get; set; }
        public enum OrderStatus
        {
            Created,
            Paid,
            Processing,
            Shipped,
            Delivered,
            Cancelled
        }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
