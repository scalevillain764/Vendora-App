using Application.DTO.CartDTO;
using Application.DTO.ProductDTO.CartDTO;
using Application.DTO.ProductDTO.StoreDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartResponseDTO>> GetMyCartAsync(Ulid UserId, CancellationToken token);
        Task<Result<ProductCartCardResponseDTO>> IncreaseQuantityAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<ProductCartCardResponseDTO>> DecreaseQuantityAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<string>> RemoveProductFromCartAsync(Ulid UserId, Ulid ProductId);
        Task<Result<ProductCartCardResponseDTO>> AddProductToCartAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<string>> ClearCartAsync(Ulid userId);
    }
}