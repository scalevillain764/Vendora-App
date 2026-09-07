using Domain.ProductStatisticsForStores;
namespace Application.DTO.ProductDTO.StatisticsDTO
{
    public record ProductStatisticsDTO(
        int LikesQuantity,
        int ReviewsLeft,
        int SoldQuantity,
        int OrdersCount,
        decimal Revenue
        )
    { 
        public ProductStatisticsDTO(ProductStatistics statistics) :
            this(
                statistics.LikesQuantity,
                statistics.ReviewsLeft,
                statistics.SoldQuantity, 
                statistics.OrdersCount, 
                statistics.Revenue) { }
    }
}