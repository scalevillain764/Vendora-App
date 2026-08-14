using Domain.ProductReviews;
namespace Application.DTO.ProductReviewDTO
{
    public record ProductReviewCreationAndChangeDTO(
        Ulid UserId,
        Ulid ProductId,
        Ulid StoreId,
        string? ReviewText,
        int Rating,
        List<string>? PhotoUrl
    );   
}