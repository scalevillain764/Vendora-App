using Application.Result;
using Vendora.Application.DTO.ProductDTO.StatisticsDTO;
namespace Application.Interfaces
{
    public interface IProductForStoreStatisticsService {
        Task<Result<ProductStatisticsDTO>> GetProductStatisticsAsync(Ulid UserId, Ulid StoreId, Ulid ProductId);
    }
}
