namespace Application.DTO.OrderDTO.OrderItemDTO
{
    public record OrderItemResponseDTO(
        string ProductName,
        decimal Price,
        int Quantity
        );
}