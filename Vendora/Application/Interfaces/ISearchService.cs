using Application.DTO.ProductDTO.StoreDTO;
using Application.Result;
using Application.DTO.SearchDTO;
namespace Application.Interfaces
{
    public interface ISearchService
    {
        Task<Result<List<ProductCardDTO>>> SearchAsync(SearchRequestDTO DTO);
    }
}