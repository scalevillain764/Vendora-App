using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.ProductReviewDTO;
using Application.Result;
using Domain.ProductReviews;
using Domain.ErrorTypes;
using Infrastructure.AppDbContexts;
using IProductReviewService = Application.Interfaces.IProductPreviewService;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
namespace Application.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly AppDbContext _context;
        public ProductReviewService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Result<ProductReviewResponseDTO>> AddProductReviewAsync(Ulid UserId, Ulid ProductId, ProductReviewCreationAndChangeDTO DTO)
        {
            bool reviewExists = await _context.ProductReviews
                .AnyAsync(r => r.UserId == UserId && r.ProductId == ProductId);

            if (reviewExists)
                return Result<ProductReviewResponseDTO>.Error("Вы уже оставили отзыв на этот товар", ErrorType.Conflict);

            var storeId = await _context.Products
                .Where(x => x.Id == ProductId)
                .Select(x => (Ulid?)x.StoreId)
                .FirstOrDefaultAsync();

            if(storeId == null)
                return Result<ProductReviewResponseDTO>.Error("Товар не найден", ErrorType.NotFound);

            bool exists = await _context.Orders.AnyAsync(x => x.UserId == UserId
                    && x.Items.Any(z => z.ProductId == ProductId));

            if (!exists)
                return Result<ProductReviewResponseDTO>.Error("Сначала необходимо купить товар", ErrorType.Forbidden);

            var review = new ProductReview(UserId, ProductId, storeId.Value, DTO.ReviewText, DTO.Rating, DTO.PhotoUrl);

            _context.ProductReviews.Add(review);

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(new ProductReviewResponseDTO(review));
        }

        public async Task<Result<ProductReviewResponseDTO>> DeleteProductReviewAsync(Ulid UserId, Ulid ReviewId)
        {
            var review = await _context.ProductReviews
                .FindAsync(ReviewId);

            if (review == null)
                return Result<ProductReviewResponseDTO>.Error("Отзыв не найден", ErrorType.Conflict);

            if (review.UserId != UserId)
                return Result<ProductReviewResponseDTO>.Error("Это не ваш отзыв", ErrorType.Forbidden);

            var DTO = new ProductReviewResponseDTO(review);
            _context.ProductReviews.Remove(review);

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(DTO);
        }

        public async Task<Result<ProductReviewResponseDTO>> EditProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewCreationAndChangeDTO DTO)
        {
            var review = await _context.ProductReviews
               .FindAsync(ReviewId);

            if (review == null)
                return Result<ProductReviewResponseDTO>.Error("Отзыв не найден", ErrorType.Conflict);

            if (review.UserId != UserId)
                return Result<ProductReviewResponseDTO>.Error("Это не ваш отзыв", ErrorType.Forbidden);

            review.ReviewText = review.ReviewText == DTO.ReviewText ? review.ReviewText : DTO.ReviewText;
            review.Rating = review.Rating == DTO.Rating ? review.Rating : DTO.Rating;

            if (DTO.PhotoUrl == null) review.PhotoUrls = null;
            else
            {
                if (review.PhotoUrls != null)
                {
                    bool areEqual = review.PhotoUrls.Count == DTO.PhotoUrl.Count
                        && new HashSet<string>(review.PhotoUrls).SetEquals(DTO.PhotoUrl);

                    review.PhotoUrls = areEqual ? review.PhotoUrls : DTO.PhotoUrl;
                }
                else
                    review.PhotoUrls = DTO.PhotoUrl;
            }

            review.UpdatedAt = DateTime.UtcNow;

            return Result<ProductReviewResponseDTO>.Success(new ProductReviewResponseDTO(review));
        }
    }
}