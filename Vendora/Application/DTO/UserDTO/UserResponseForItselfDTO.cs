using Domain.Users;
namespace Application.DTO.UserDTO
{
    public record UserResponseForItselfDTO(
        string ProfileName,
        string FirstName,
        string LastName,
        string Gender,
        string? AvatarUrl,
        string? Email,
        string? Phone,
        decimal Balance,
        int OrdersMade,
        int ReviewsLeft
    )
    {
        public UserResponseForItselfDTO(User user, int productsBought, int reviewsLeft) :
            this(user.ProfileName, user.FirstName, user.LastName, user.UserGender.ToString(),
                user.AvatarUrl, user.Email, user.Phone, user.Balance, productsBought, reviewsLeft)
        { }
    };
}