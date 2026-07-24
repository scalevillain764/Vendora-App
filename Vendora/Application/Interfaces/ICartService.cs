using Application.Result;
using Application.DTO.CartDTO;
using Application.DTO.ProductDTO.StoreDTO;
namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartResponseDTO>> GetMyCartAsync(Ulid UserId);
        Task<Result<CartResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid CartItemId);
        Task<Result<CartResponseDTO>> DecreaseQuantityAsync(Ulid UserId, Ulid CartItemId);
        Task<Result<CartResponseDTO>> RemoveCartItemAsync(Ulid UserId, Ulid CartItemId);
        Task<Result<CartResponseDTO>> AddProductToCartAsync(Ulid UserId, Ulid ProductId);
    }
}