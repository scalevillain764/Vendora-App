using Domain.Users;
namespace Application.DTO.AuthDTO
{
    public record UserLogInResponseDTO(
        Ulid UserId,
        string AccessToken,
        string ProfileName,
        string FirstName,
        string LastName
    )
    {
        public UserLogInResponseDTO(string accessToken, User user) :
            this(user.Id, accessToken, user.ProfileName, user.FirstName, user.LastName)
        { }
    };
}