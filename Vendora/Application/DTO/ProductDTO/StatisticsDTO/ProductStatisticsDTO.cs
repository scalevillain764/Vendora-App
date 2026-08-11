using Domain.ProductStatisticsForStores;
namespace Vendora.Application.DTO.ProductDTO.StatisticsDTO
{
    public record ProductStatisticsDTO(
        int SoldQuality,
        int OrdersCount,
        decimal Revenue
        )
    { 
        public ProductStatisticsDTO(ProductStatistics statistics) :
            this(statistics.SoldQuality, statistics.OrdersCount, statistics.Revenue) { }
    }
}