using Application.DTO.UserDTO;
using Application.Result;
using Microsoft.AspNetCore.Http;
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<string>> DeleteMyAccountAsync(Ulid UserId, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId, CancellationToken token);
        Task<Result<UserResponseForOthersDTO>> GetUserAsync(Ulid UserId, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserProfileNameAsync(Ulid UserId, UserChangeProfileNameDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserFirstNameAsync(Ulid UserId, UserChangeFirstNameDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLastNameAsync(Ulid UserId, UserChangeLastNameDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserEmailAsync(Ulid UserId, UserChangeEmailDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserPhoneAsync(Ulid UserId, UserChangePhoneDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserGenderAsync(Ulid UserId, UserChangeGenderDTO DTO, CancellationToken token);
        Task<Result<UserResponseForItselfDTO>> ChangeUserProfilePictureAsync(Ulid UserId, IFormFile? file, CancellationToken token);
    }
}