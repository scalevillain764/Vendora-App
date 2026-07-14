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
        decimal Balance
    )
    {
        public UserResponseForItselfDTO(User user) :
            this(user.ProfileName, user.FirstName, user.LastName, user.UserGender.ToString(),
                user.AvatarUrl, user.Email, user.Phone, user.Balance)
        { }
    };
}