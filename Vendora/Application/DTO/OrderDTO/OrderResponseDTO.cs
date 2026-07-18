using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using Application.DTO.OrderDTO.OrderItemDTO;
namespace Application.DTO.OrderDTO
{
    public record OrderResponseDTO(
        Ulid OrderId,
        Ulid UserId,
        decimal TotalPrice,
        List<OrderItemResponseDTO> Items
    );
}