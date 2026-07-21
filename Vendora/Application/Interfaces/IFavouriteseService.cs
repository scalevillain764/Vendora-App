using Application.Result;
using Application.DTO.FavouriteDTO;
using Application.DTO.ProductDTO;
using Application.DTO.ProductDTO.StoreDTO;
namespace Application.Interfaces
{
    public interface IFavouriteService
    {
        Task<Result<FavoriteResponseDTO>> AddToFavourite(Ulid UserId, Ulid ProductId);
        Task<Result<FavoriteResponseDTO>> RemoveFromFavourite(Ulid UserId, Ulid ProductId);
        Task<Result<List<ProductCardDTO>>> GetFavourites(Ulid UserId);
    }
}