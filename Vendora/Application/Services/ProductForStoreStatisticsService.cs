using Application.Result;
using Infrastructure.AppDbContexts;
using Microsoft.IdentityModel.Tokens;
using Vendora.Application.DTO.ProductDTO.StatisticsDTO;
using Domain.ErrorTypes;
using IProductForStoreStatisticsService = Application.Interfaces.IProductForStoreStatisticsService;
using Application.DTO.ProductDTO.StoreDTO;
namespace Application.Services
{
    public class ProductForStoreStatisticsService : IProductForStoreStatisticsService
    {
        private readonly AppDbContext _context;
        public ProductForStoreStatisticsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<ProductStatisticsDTO>> GetProductStatisticsAsync(Ulid UserId, Ulid StoreId, Ulid ProductId)
        {
            var store = await _context.Stores
                .FindAsync(StoreId);

            if (store == null)
                return Result<ProductStatisticsDTO>.Error("Магазин не найден", ErrorType.Forbidden);

            if (store.SellerId != UserId)
                return Result<ProductStatisticsDTO>.Error("Это не ваш магазин", ErrorType.Forbidden);

            var product = await _context.Products
                .FindAsync(ProductId);

            if (product == null)
                return Result<ProductStatisticsDTO>.Error("Продукт не найден", ErrorType.NotFound);

            if (product.StoreId != StoreId)
                return Result<ProductStatisticsDTO>.Error("Продукт не принадлежит вашему магазину", ErrorType.Conflict);

            var statistics = await _context.ProductStatistics
                .FindAsync(ProductId);

            if (statistics == null)
                return Result<ProductStatisticsDTO>.Error("Увы, статистика не найдена", ErrorType.NotFound);

            return Result<ProductStatisticsDTO>.Success(new ProductStatisticsDTO(statistics));
        }
    }
}