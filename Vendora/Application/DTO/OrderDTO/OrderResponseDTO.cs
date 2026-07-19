using Application.DTO.OrderDTO.OrderItemDTO;
using Domain.Orders;
namespace Application.DTO.OrderDTO
{
    public record OrderResponseDTO(
        Ulid OrderId,
        decimal TotalPrice,
        string Status,
        DateTime CreatedAt,
        List<OrderItemResponseDTO> Items
    )
    {
        public OrderResponseDTO(Order order, List<OrderItemResponseDTO> It)
            : this(order.Id, order.TotalPrice, order.Status.ToString(), order.CreatedAt, It)
        { }
    }
}
