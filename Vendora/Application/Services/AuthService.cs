using _authServiceInterface;
using _error_types;
using _result;
using _user;
using _userLogInDTO;
using _userLogInResponseDTO;
using _userRegistrationDTO;
using _userRegistrationResponseDTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using _appDbContext;
using Microsoft.EntityFrameworkCore;
namespace _authService
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public AuthService(IConfiguration configuration, IHttpContextAccessor accessor, AppDbContext context)
        {
            _configuration = configuration;
            _httpContextAccessor = accessor;
            _context = context;
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

            string decoded_key = _configuration["secret_key"];
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

        public async Task<Result<UserLogInResponseDTO>> RefreshAsync()
        {
            string existingRefreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(existingRefreshToken))
                return Result<UserLogInResponseDTO>.Error("Куки пусты", ErrorType.Unauthorized);

            string existingRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(existingRefreshToken);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => BCrypt.Net.BCrypt.Verify(existingRefreshTokenHash, x.RefreshTokenHash));

            if (user == null)
                return Result<UserLogInResponseDTO>.Error("Пользователь не найден", ErrorType.Unauthorized);

            if (user.RefreshTokenExpiresAt != null)
            {
                if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
                    return Result<UserLogInResponseDTO>.Error("Сессия истекла", ErrorType.Unauthorized);
            }
            else
                return Result<UserLogInResponseDTO>.Error("Ошибка сессии", ErrorType.Unauthorized);

            string AccessToken = AppendCookiesAndGetAccessToken(user);

            await _context.SaveChangesAsync();
            return Result<UserLogInResponseDTO>.Success(new UserLogInResponseDTO(AccessToken, user));
        }
    }
}