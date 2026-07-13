using _user;
namespace _userLogInResponseDTO
{
    public class UserLogInResponseDTO
    {
        public Ulid UserId { get; set; }
        public string AccessToken { get; set; }
        public string ProfileName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserLogInResponseDTO(string accessToken, User user)
        {
            UserId = user.Id;
            AccessToken = accessToken;
            ProfileName = user.ProfileName;
            FirstName = user.FirstName;
            LastName = user.LastName;
        }
    }
}