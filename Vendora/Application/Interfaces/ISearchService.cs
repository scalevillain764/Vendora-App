using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.SearchDTO;
using Application.PagedResponse;
using Application.Result;
namespace Application.Interfaces
{
    public interface ISearchService
    {
        Task<Result<PagedResponse<ProductCardDTO>>> SearchAsync(Ulid UserId, SearchRequestDTO DTO);
    }
}