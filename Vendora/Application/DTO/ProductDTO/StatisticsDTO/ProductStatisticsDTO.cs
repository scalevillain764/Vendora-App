using Domain.ProductStatisticsForStores;
namespace Application.DTO.ProductDTO.StatisticsDTO
{
    public record ProductStatisticsDTO(
        int SoldQuantity,
        int OrdersCount,
        decimal Revenue
        )
    { 
        public ProductStatisticsDTO(ProductStatistics statistics) :
            this(statistics.SoldQuantity, statistics.OrdersCount, statistics.Revenue) { }
    }
}