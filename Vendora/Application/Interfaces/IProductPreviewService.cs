using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.ProductReviewDTO;
using Application.Result;
using Domain.ProductReviews;
namespace Application.Interfaces
{
    public interface IProductPreviewService
    {
        Task<Result<ProductReviewResponseDTO>> AddProductReviewAsync(Ulid UserId, Ulid ProductId, ProductReviewCreationAndChangeDTO DTO);
        Task<Result<ProductReviewResponseDTO>> DeleteProductReviewAsync(Ulid UserId, Ulid ReviewId);
        Task<Result<ProductReviewResponseDTO>> EditProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewCreationAndChangeDTO DTO);
        // Add Repl
    }
}