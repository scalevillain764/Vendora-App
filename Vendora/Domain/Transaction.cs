using Domain.Orders;
namespace Domain.Transactions
{
    public class Transaction
    {
        public Ulid Id { get; set; }
        
        // references
        public Ulid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string? ExternalPaymentId { get; set; }
        public decimal Amount { get; set; }

        public enum PaymentMethod { Balance = 1, YOOKassa = 2 };
        public PaymentMethod Method { get; set; }

        public enum PaymentStatus {Pending = 1, Success = 2, Failed = 3};
        public PaymentStatus Status { get; set; }

        public Transaction(Ulid orderId, string? externalPaymentId, decimal amount, PaymentMethod method)
        {
            Id = Ulid.NewUlid();
            OrderId = orderId;
            ExternalPaymentId = externalPaymentId;
            Amount = amount;
            Method = method;
            Status = PaymentStatus.Pending;
        }
    }
}