using Application.Result;
using Infrastructure.AppDbContexts;
using Microsoft.IdentityModel.Tokens;
using Application.DTO.ProductDTO.StatisticsDTO;
using Domain.ErrorTypes;
using IProductForStoreStatisticsService = Application.Interfaces.IProductStatisticsService;
using Application.DTO.ProductDTO.StoreDTO;
using Microsoft.EntityFrameworkCore;
namespace Application.Services
{
    public class ProductStatisticsService : IProductForStoreStatisticsService
    {
        private readonly AppDbContext _context;
        public ProductStatisticsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<ProductStatisticsDTO>> GetProductStatisticsAsync(Ulid UserId, Ulid StoreId, Ulid ProductId)
        {
            var statistics = await _context.Products
                .Include(x => x.Statistics)
                .Where(x => x.Store.SellerId == UserId 
                && x.StoreId == StoreId
                && x.Id == ProductId)
                .Select(x => x.Statistics)
                .FirstOrDefaultAsync();

            if (statistics == null)
                return Result<ProductStatisticsDTO>.Error("Увы, статистика не найдена", ErrorType.NotFound);

            return Result<ProductStatisticsDTO>.Success(new ProductStatisticsDTO(statistics));
        }
    }
}