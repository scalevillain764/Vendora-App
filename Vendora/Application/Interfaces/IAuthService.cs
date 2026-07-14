using Application.Result;
using Domain.Users;
using Application.DTO.AuthDTO;
namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<UserRegistrationResponseDTO>> Registr(UserRegistrationDTO DTO);
        Task<Result<UserLogInResponseDTO>> LogIn(UserLogInDTO DTO);
    }
}