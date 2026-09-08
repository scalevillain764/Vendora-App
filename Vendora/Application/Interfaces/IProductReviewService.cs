using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.ProductReviewDTO;
using Application.Result;
using Domain.ProductReviews;
namespace Application.Interfaces
{
    public interface IProductReviewService
    {
        Task<Result<ProductReviewResponseDTO>> AddProductReviewAsync(Ulid UserId, Ulid ProductId, ProductReviewCreationAndChangeDTO DTO, CancellationToken token);
        Task<Result<ProductReviewResponseDTO>> DeleteProductReviewAsync(Ulid UserId, Ulid ReviewId, CancellationToken token);
        Task<Result<ProductReviewResponseDTO>> EditProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewCreationAndChangeDTO DTO, CancellationToken token);
        Task<Result<List<ProductReviewResponseDTO>>> GetProductReviewsAsync(Ulid UserId, Ulid ProductId, CancellationToken token);
        Task<Result<ProductReviewResponseDTO>> ReplyProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewSellerReplyDTO DTO, CancellationToken token);
    }
}