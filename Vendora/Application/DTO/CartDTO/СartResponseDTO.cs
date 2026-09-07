using Application.DTO.ProductDTO.CartDTO;
namespace Application.DTO.CartDTO
{
    public record CartResponseDTO(
        Ulid UserId,
        List<ProductCartCardResponseDTO> cartItems,
        int TotalQuantity,
        decimal TotalPrice
        );
}