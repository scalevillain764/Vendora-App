using Application.DTO.OrderDTO.OrderItemDTO;
namespace Application.DTO.OrderDTO
{
    public record OrderResponseDTO(
    Ulid OrderId,
    decimal TotalPrice,
    string Status,
    DateTime CreatedAt,
    List<OrderItemResponseDTO> Items
    );
}
