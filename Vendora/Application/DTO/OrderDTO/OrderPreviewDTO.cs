using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using Application.DTO.OrderDTO.OrderItemDTO;
namespace Application.DTO.OrderDTO
{
    public record OrderPreviewDTO(
        Ulid OrderId,
        decimal TotalPrice,
        string OrderStatus,
        List<OrderItemResponseDTO> Items
    )
    {
        public OrderPreviewDTO(Order order, List<OrderItemResponseDTO> It)
            : this(order.Id, order.TotalPrice, order.Status.ToString(), It)
        { }
    }
}