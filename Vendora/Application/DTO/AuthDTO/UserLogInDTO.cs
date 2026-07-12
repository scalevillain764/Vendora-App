namespace _userLogInDTO
{
    public class UserLogInDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public UserLogInDTO(string Login, string Password)
        {
            this.Password = Password;
            this.Login = Login;
        }
    }
}