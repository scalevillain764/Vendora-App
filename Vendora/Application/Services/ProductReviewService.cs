using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.ProductReviewDTO;
using Application.Result;
using Domain.ProductReviews;
using Domain.ErrorTypes;
using Infrastructure.AppDbContexts;
using IProductReviewService = Application.Interfaces.IProductReviewService;
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

            var order = await _context.OrderItems
                .Where(x => x.ProductId == ProductId)
                .Select(x => x.Order)
                .FirstOrDefaultAsync(x => x.UserId == UserId);

            if (order == null || order.Status != Domain.Orders.Order.OrderStatus.Completed)
                return Result<ProductReviewResponseDTO>.Error("Сначала приорбретите товар", ErrorType.Forbidden);

            var review = new ProductReview(UserId, ProductId, storeId.Value, DTO.ReviewText, DTO.Rating, DTO.PhotoUrl);

            _context.ProductReviews.Add(review);

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(new ProductReviewResponseDTO(review, false));
        }

        public async Task<Result<ProductReviewResponseDTO>> DeleteProductReviewAsync(Ulid UserId, Ulid ReviewId)
        {
            var review = await _context.ProductReviews
                .Include(x => x.store)
                .FirstOrDefaultAsync(x => x.Id == ReviewId);

            if (review == null)
                return Result<ProductReviewResponseDTO>.Error("Отзыв не найден", ErrorType.Conflict);

            if (review.UserId != UserId && review.store.SellerId != UserId /*удаление админом магазина*/)
                return Result<ProductReviewResponseDTO>.Error("Это не ваш отзыв", ErrorType.Forbidden);

            var DTO = new ProductReviewResponseDTO(review, false);
            _context.ProductReviews.Remove(review);

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(DTO);
        }

        public async Task<Result<ProductReviewResponseDTO>> EditProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewCreationAndChangeDTO DTO)
        {
            var review = await _context.ProductReviews
               .FindAsync(ReviewId);

            if (review == null)
                return Result<ProductReviewResponseDTO>.Error("Отзыв не найден", ErrorType.NotFound);

            if (review.UserId != UserId)
                return Result<ProductReviewResponseDTO>.Error("Это не ваш отзыв", ErrorType.Forbidden);

            review.ReviewText = DTO.ReviewText;
            review.Rating = DTO.Rating;
            review.PhotoUrls = DTO.PhotoUrl;
            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(new ProductReviewResponseDTO(review, false));
        }

        public async Task<Result<List<ProductReviewResponseDTO>>> GetProductReviewsAsync(Ulid UserId, Ulid ProductId)
        {
            var product = await _context.Products
                    .Include(x => x.Store)
                .FirstOrDefaultAsync(x => x.Id == ProductId);

            if (product == null)
                return Result<List<ProductReviewResponseDTO>>.Error("Товар не найден", ErrorType.NotFound);

            bool canReply = product.Store.SellerId == UserId;

            var rez = await _context.ProductReviews
                .Where(x => x.ProductId == ProductId)
                .Select(x => new ProductReviewResponseDTO(x, canReply))
                .ToListAsync();

            return Result<List<ProductReviewResponseDTO>>.Success(rez);
        }

        public async Task<Result<ProductReviewResponseDTO>> ReplyProductReviewAsync(Ulid UserId, Ulid ReviewId, ProductReviewSellerReplyDTO DTO)
        {
            var review = await _context.ProductReviews
              .Include(x => x.store)
              .FirstOrDefaultAsync(x => x.Id == ReviewId);

            if (review == null)
                return Result<ProductReviewResponseDTO>.Error("Отзыв не найден", ErrorType.NotFound);

            if (review.store.SellerId != UserId)
                return Result<ProductReviewResponseDTO>.Error("Вы не можете ответить", ErrorType.Conflict);

            review.SellerReply = DTO.SellerReply;

            await _context.SaveChangesAsync();

            return Result<ProductReviewResponseDTO>.Success(new ProductReviewResponseDTO(review, false));
        }
    }
}