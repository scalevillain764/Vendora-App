using Domain.OrderItems;
using Domain.Orders;
using Domain.Users;
using Application.DTO.OrderDTO.OrderItemDTO;
namespace Application.DTO.OrderDTO
{
    public record OrderResponseDTO (
        Ulid OrderId,
        Ulid UserId,
        string UserProfileName,
        decimal TotalPrice,
        List<OrderItemResponseDTO> Items)
    {
        public OrderResponseDTO (Order order, User user, List<OrderItemResponseDTO> ProductNames) :
            this(order.Id, user.Id, user.ProfileName, order.TotalPrice, ProductNames)
        { }
    }
}