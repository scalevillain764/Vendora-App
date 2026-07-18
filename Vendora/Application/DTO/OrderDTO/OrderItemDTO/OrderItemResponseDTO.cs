using Domain.OrderItems;
namespace Application.DTO.OrderDTO.OrderItemDTO
{
    public record OrderItemResponseDTO(
        string ProductName,
        decimal TotalPrice,
        int Quantity
        )
    {
        public OrderItemResponseDTO(OrderItem orderItem) :
            this(orderItem.Product.Name, orderItem.PricePerUnit * orderItem.Quantity, orderItem.Quantity)
        { }
    }
}