using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCardDTO(
        Ulid ProductId,
        string StoreName,
        string Name,
        decimal Price,
        string? ShortDescription,
        string? PreviewUrl,
        bool IsFavourite,
        double AverageRating,
        int RatingsAmount
    )
    {
        public ProductCardDTO(Product product, bool isFavourite) : 
            this(product.Id, 
                product.Store.Name, 
                product.Name, 
                product.Price, 
                product.ShortDescription, 
                product.PreviewUrl, 
                isFavourite,
                product.ProductReviews.Count != 0 ? product.ProductReviews.Average(x => x.Rating) : 0,
                product.ProductReviews.Count) { }
    }
}