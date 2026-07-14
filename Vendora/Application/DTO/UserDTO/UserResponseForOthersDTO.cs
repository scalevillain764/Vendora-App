
namespace UserDTOForOthers
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
        public UserResponseForOthersDTO()
    }
}