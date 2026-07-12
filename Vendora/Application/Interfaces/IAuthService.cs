using _result;
using _user;
using _userLogInDTO;
using _userRegistrationDTO;
using _userRegistrationResponseDTO;
using _userLogInResponseDTO;
namespace _authServiceInterface
{
    public interface IAuthService
    {
        Task<Result<UserRegistationResponseDTO>> Registr(UserRegistrationDTO DTO);
        Task<Result<UserLogInResponseDTO>> LogIn(UserLogInDTO DTO);
    }
}