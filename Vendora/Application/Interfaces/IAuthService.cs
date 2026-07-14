using Application.DTO.AuthDTO;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.Users;
namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<UserRegistrationResponseDTO>> RegistrAsync(UserRegistrationDTO DTO);
        Task<Result<AuthResponseDTO>> LogInAsync(UserLogInDTO DTO);
        Task<Result<AuthResponseDTO>> ChangeUserPasswordAsync(Ulid UserId, UserChangePasswordDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLoginAsync(Ulid UserId, UserChangeLoginDTO DTO);
    }
}