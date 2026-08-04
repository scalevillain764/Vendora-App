using Domain.Products;
using Application.DTO.ProductReviewDTO;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductResponseDTO(
         Ulid ProductId,
         Ulid StoreId,
         string Category,
         string Name,
         string? Description,
         decimal Price,
         int Quantity,
         string? PreviewUrl,
         long Article,
         List<ProductReviewResponseDTO> Reviews,
         double Average_rating
    )
    {
        public ProductResponseDTO(Product pr) :
            this(pr.Id, pr.StoreId,
                pr.Category.ToString(),
                pr.Name, pr.Description,
                pr.Price, pr.Quantity,
                pr.PreviewUrl, pr.Article,
                pr.ProductReviews.Select(x => new ProductReviewResponseDTO(x)).ToList(),
                pr.ProductReviews.Average(x => x.Rating)
            )
        { }
    }
}