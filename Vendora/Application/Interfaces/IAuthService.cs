using Application.DTO.AuthDTO;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.Users;
namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<UserRegistrationResponseDTO>> RegistrAsync(UserRegistrationDTO DTO, CancellationToken token);
        Task<Result<AuthResponseDTO>> LogInAsync(UserLogInDTO DTO, CancellationToken token);
        Task<Result<AuthResponseDTO>> RefreshAsync(Ulid userId, CancellationToken token);
        Task<Result<AuthResponseDTO>> ChangeUserPasswordAsync(Ulid UserId, UserChangePasswordDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLoginAsync(Ulid UserId, UserChangeLoginDTO DTO, CancellationToken token);
    }
}