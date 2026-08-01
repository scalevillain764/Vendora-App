using Domain.ProductReviews;
namespace Application.DTO.ProductReviewDTO
{
    public record ProductReviewResponseDTO(Ulid Id, string? UserAvatarPicture, string UserProfileName,
        int rating, List<string>? Photos, string? SellerReply,
        DateTime CreateAt, DateTime? UpdatedAt)
    {
        public ProductReviewResponseDTO(ProductReview review) 
            : this(review.Id, review.user.AvatarUrl, review.user.ProfileName, review.Rating,
                  review.PhotoUrls, review.SellerReply, review.CreatedAt, review.UpdatedAt)
        { }
    }
}