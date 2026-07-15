using Domain.Carts;
using Domain.Stores;
namespace Domain.Users
{
    public class User
    {
        public Ulid Id { get; private set; }
        public string ProfileName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Login { get; set; } 
        public string PasswordHash { get; set; }
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        public enum Gender { Male = 0, Female = 1, Undefined = 2};     
        public Gender UserGender { get; set; }

        public string? AvatarUrl { get;  set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsDeleted { get; set; }

        public decimal Balance { get; set; }

        // references 
        public Store? Store { get; set; } = null; // np

        public Cart? Cart { get; set; } = null; // np
        
        public User(string profileName, string firstName, string lastName, string login, string passwordHash, string refreshTokenHash, DateTime refreshTokenExpiresAt, Gender userGender, string? avatarUrl, string? email, string? phone)
        {
            Id = Ulid.NewUlid();
            ProfileName = profileName;
            FirstName = firstName;
            LastName = lastName;
            Login = login;
            PasswordHash = passwordHash;
            RefreshTokenHash = refreshTokenHash;
            RefreshTokenExpiresAt = refreshTokenExpiresAt;
            UserGender = userGender;
            AvatarUrl = avatarUrl;
            Email = email;
            Phone = phone;
            IsDeleted = false;
            Balance = 0;
        }

        public User(string login, string passwordHash)
        {
            Id = Ulid.NewUlid();
            ProfileName = login;
            FirstName = string.Empty;
            LastName = string.Empty;
            Login = login;
            PasswordHash = passwordHash;
            RefreshTokenHash = string.Empty;
            RefreshTokenExpiresAt = null;
            UserGender = Gender.Undefined;
            AvatarUrl = null;
            Email = null;
            Phone = null;
            IsDeleted = false;
            Balance = 0;
        }
    }
}