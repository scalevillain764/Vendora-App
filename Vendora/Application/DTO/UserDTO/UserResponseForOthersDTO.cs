using Domain.Users;
namespace Application.DTO.UserDTO
{
    public record UserResponseForOthersDTO
    (
        string ProfileName,
        string FirstName,
        string LastName,
        string Gender,
        string? AvatarUrl,
        string? Email,
        string? Phone
    )
    {
        public UserResponseForOthersDTO(User user) : 
            this(user.ProfileName, user.FirstName, user.LastName, user.UserGender.ToString(), user.AvatarUrl, user.Email, user.Email) 
        { }
    }
}