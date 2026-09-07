using Domain.Products;
namespace Domain.ProductStatisticsForStores
{
    public class ProductStatistics
    {
        public Ulid ProductId { get; private set; }
        public Product? Product { get; set; } = null;
        public int LikesQuantity { get; set; }
        public int ReviewsLeft { get; set; }
        public int SoldQuantity { get; set; }
        public int OrdersCount { get; set; }
        public decimal Revenue { get; set; }
        public ProductStatistics(Ulid productId)
        {
            ProductId = productId;
            LikesQuantity = 0;
            ReviewsLeft = 0;
            SoldQuantity = 0;
            OrdersCount = 0;
            Revenue = 0;
        }
    }
}