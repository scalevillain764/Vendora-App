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
         double Average_rating,
         bool IsOwner
    )
    {
        public ProductResponseDTO(Product pr, bool isOwner) :
            this(pr.Id, pr.StoreId,
                pr.Category.ToString(),
                pr.Name, pr.Description,
                pr.Price, pr.Quantity,
                pr.PreviewUrl, pr.Article,
                pr.ProductReviews.Average(x => x.Rating),
                isOwner
            )
        { }
    }
}