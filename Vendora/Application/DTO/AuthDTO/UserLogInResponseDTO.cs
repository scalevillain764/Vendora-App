namespace _userLogInResponseDTO
{
    public class UserLogInResponseDTO
    {
        public Ulid UserId { get; set; }
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string ProfileName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserLogInResponseDTO(Ulid userId, string accessToken, string? refreshToken, string profileName, string firstName, string lastName)
        {
            UserId = userId;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ProfileName = profileName;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}