using Application.Result;
using Domain.Users;
using Application.DTO.AuthDTO.UserLogInDTOS;
using Application.DTO.AuthDTO.UserRegistrationDTOS;
using Application.DTO.AuthDTO.UserRegistrationResponseDTOS;
using Application.DTO.AuthDTO.UserLogInResponseDTOS;
namespace Application.Interfaces.AuthServiceInterface
{
    public interface IAuthService
    {
        Task<Result<UserRegistrationResponseDTO>> Registr(UserRegistrationDTO DTO);
        Task<Result<UserLogInResponseDTO>> LogIn(UserLogInDTO DTO);
    }
}