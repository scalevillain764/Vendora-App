using Domain.OrderItems;
using Domain.Users;
using Domain.Transactions;
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
            Pending,    // Waiting for payment
            PaymentCompleted,  
            PaymentFailed,
            Refunded,    // Money returned 
            InDelivery,
            ReadyForPickup,
            Completed, // Product is given and paid
            Cancelled
        }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItem> Items { get; set; } = [];
        public ICollection<Transaction> Transactions { get; set; } = [];

        public Order(Ulid userId, decimal totalPrice)
        {
            Id = Ulid.NewUlid();
            UserId = userId;
            TotalPrice = totalPrice;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
