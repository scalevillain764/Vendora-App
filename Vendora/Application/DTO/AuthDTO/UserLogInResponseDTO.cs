using _user;
namespace _userLogInResponseDTO
{
    public record UserLogInResponseDTO(
        Ulid UserId,
        string AccessToken,
        string ProfileName,
        string FirstName,
        string LastName
    );
}