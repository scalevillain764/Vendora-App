using Application.Result;
using Application.DTO.FavouriteDTO;
using Application.DTO.ProductDTO;
using Application.DTO.ProductDTO.StoreDTO;
namespace Application.Interfaces
{
    public interface IFavouriteService
    {
        Task<Result<FavoriteResponseDTO>> AddToFavouriteAsync(Ulid UserId, Ulid ProductId);
        Task<Result<FavoriteResponseDTO>> RemoveFromFavouriteAsync(Ulid UserId, Ulid ProductId);
        Task<Result<List<ProductCardDTO>>> GetFavouritesByIdAsync(Ulid UserId);
    }
}