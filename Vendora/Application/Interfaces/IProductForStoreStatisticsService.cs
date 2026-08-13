using Application.Result;
using Application.DTO.ProductDTO.StatisticsDTO;
namespace Application.Interfaces
{
    public interface IProductStatisticsService {
        Task<Result<ProductStatisticsDTO>> GetProductStatisticsAsync(Ulid UserId, Ulid StoreId, Ulid ProductId);
    }
}
