using Domain.OrderItems;
using Domain.Orders;
namespace Application.DTO.OrderDTO
{
    public record OrderResponseDTO (
        Ulid OrderId,
        Ulid UserId,
        string UserProfileName,
        string TotalPrice,
        string Quantity,
        List<string> ProductNames)
    {
        public OrderResponseDTO ()
    }
}