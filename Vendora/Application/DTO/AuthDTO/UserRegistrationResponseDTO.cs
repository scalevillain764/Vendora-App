namespace _userRegistrationResponseDTO
{
    public class UserRegistationResponseDTO
    {
        public Ulid UserId { get; set; }
        public string Login { get; set; }
        public UserRegistationResponseDTO(Ulid UserId, string Login)
        {
            this.UserId = UserId;
            this.Login = Login;
        }
    }
}