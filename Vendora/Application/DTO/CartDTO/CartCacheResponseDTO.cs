using Application.DTO.ProductDTO.CartDTO;
namespace Application.DTO.CartDTO
{
    public record CartCacheResponseDTO(Ulid UserId, Dictionary<Ulid, int>? CartItems = null)
    {
        public Dictionary<Ulid, int> CartItems { get; init; } = CartItems ?? new();
    }
}