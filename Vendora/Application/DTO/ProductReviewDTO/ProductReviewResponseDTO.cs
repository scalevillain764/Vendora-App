using Domain.ProductReviews;
namespace Application.DTO.ProductReviewDTO
{
    public record ProductReviewResponseDTO(Ulid Id, string? UserAvatarPicture, string UserProfileName,
        int rating, List<string>? Photos, string ReviewText, string? SellerReply, bool CanSellerReply,
        DateTime CreateAt, DateTime? UpdatedAt)
    {
        public ProductReviewResponseDTO(ProductReview review, bool CanSellerReply) 
            : this(review.Id, review.user.AvatarUrl, review.user.ProfileName, review.Rating,
                  review.PhotoUrls, review.ReviewText,
                  review.SellerReply, CanSellerReply, review.CreatedAt, review.UpdatedAt)
        { }
    }
}