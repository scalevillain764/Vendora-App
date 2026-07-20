using Application.DTO.AuthDTO;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using IAuthService = Application.Interfaces.IAuthService;
namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public AuthService(IHttpContextAccessor accessor, AppDbContext context)
        {
            _httpContextAccessor = accessor;
            _context = context;
        }

        public async Task<Result<UserRegistrationResponseDTO>> RegistrAsync(UserRegistrationDTO DTO)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Login == DTO.Login); // потом оптимизировать

            if(existingUser != null)
                return Result<UserRegistrationResponseDTO>.Error("Пользователь с таким логином уже существует", ErrorType.Conflict);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(DTO.Password, workFactor: 11);

            var user = new User(DTO.Login, passwordHash);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return Result<UserRegistrationResponseDTO>.Success(new UserRegistrationResponseDTO(user.Id, user.Login));
        }
        public async Task<Result<AuthResponseDTO>> LogInAsync(UserLogInDTO DTO)
        {
            var existingUser = await _context.Users
             .FirstOrDefaultAsync(x => x.Login == DTO.Login); // потом оптимизировать

            if (existingUser == null)
                return Result<AuthResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            if(!BCrypt.Net.BCrypt.Verify(DTO.Password, existingUser.PasswordHash))
                return Result<AuthResponseDTO>.Error("Неверный пароль", ErrorType.Conflict);

            string accessToken = AppendCookiesAndGetAccessToken(existingUser);
            await _context.SaveChangesAsync();

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO(existingUser.Id, accessToken));
        }

        private string AppendCookiesAndGetAccessToken(User user)
        {
            string accessToken = CreateAccessToken(user.Id, user.ProfileName);
            string refreshToken = CreateRefreshToken();

            string refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            return accessToken;
        }

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string CreateAccessToken(Ulid Id, string UserName)
        {
            Claim[] claims =
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Name, UserName)
            };

            string decoded_key = Environment.GetEnvironmentVariable("SECRET_KEY");
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(decoded_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                   issuer: "VendoraAuthServer",
                   audience: "VendoraAudience",
                   claims: claims,
                   expires: DateTime.UtcNow.AddMinutes(15),
                   signingCredentials: creds
                   );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<AuthResponseDTO>> RefreshAsync(Ulid userId)
        {
            string existingRefreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(existingRefreshToken))
                return Result<AuthResponseDTO>.Error("Куки пусты", ErrorType.Unauthorized);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return Result<AuthResponseDTO>.Error("Пользователь не найден", ErrorType.Unauthorized);

            if (user.RefreshTokenExpiresAt == null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
                return Result<AuthResponseDTO>.Error("Сессия истекла", ErrorType.Unauthorized);

            if (!BCrypt.Net.BCrypt.Verify(existingRefreshToken, user.RefreshTokenHash))
                return Result<AuthResponseDTO>.Error("Невалидный токен сессии", ErrorType.Unauthorized);

            string AccessToken = AppendCookiesAndGetAccessToken(user);

            await _context.SaveChangesAsync();
            return Result<AuthResponseDTO>.Success(new AuthResponseDTO(user.Id, AccessToken));
        }

        public async Task<Result<AuthResponseDTO>> ChangeUserPasswordAsync(Ulid UserId, UserChangePasswordDTO DTO)
        {
            var user = await _context.Users
              .FindAsync(UserId);

            if (user == null)
                return Result<AuthResponseDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            if (!BCrypt.Net.BCrypt.Verify(DTO.OldPassword, user.PasswordHash))
                return Result<AuthResponseDTO>.Error("Старый пароль не подходит", ErrorType.Validation);

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(DTO.NewPassword, workFactor: 11);
            user.PasswordHash = newPasswordHash;

            string newAccess = AppendCookiesAndGetAccessToken(user);

            await _context.SaveChangesAsync();

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO(user.Id, newAccess));
        }

        public async Task<Result<UserResponseForItselfDTO>> ChangeUserLoginAsync(Ulid UserId, UserChangeLoginDTO DTO)
        {
            var user = await _context.Users
               .FindAsync(UserId);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            if (!BCrypt.Net.BCrypt.Verify(DTO.Password, user.PasswordHash))
                return Result<UserResponseForItselfDTO>.Error("Неверный пароль", ErrorType.Validation);

            bool loginExists = await _context.Users
                .AnyAsync(x => x.Login == DTO.Login && x.Id != UserId);

            if (loginExists)
                return Result<UserResponseForItselfDTO>.Error("Такой логин уже существует", ErrorType.Conflict);

            user.Login = DTO.Login;

            await _context.SaveChangesAsync();

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user));
        }
    }
}