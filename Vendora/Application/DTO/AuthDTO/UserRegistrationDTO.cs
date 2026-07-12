namespace _userRegistrationDTO
{
    public class UserRegistrationDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRegistrationDTO(string Login, string Password)
        {
            this.Password = Password;
            this.Login = Login;
        }
    }
}