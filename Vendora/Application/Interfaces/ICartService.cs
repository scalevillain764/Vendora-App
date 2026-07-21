using Application.Result;
using Application.DTO.ProductDTO.CartDTO;
namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<List<ProductCartCardResponseDTO>>> GetMyCartAsync(Ulid UserId);
        Task<Result<ProductCartCardResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid CartItemId);
        Task<Result<ProductCartCardResponseDTO>> DencreaseQuantityAsync(Ulid UserId, Ulid CartItemId);
        Task<Result<ProductCartCardResponseDTO>> DencreaseQuantityAsync(Ulid UserId, Ulid CartItemId);
    }
}