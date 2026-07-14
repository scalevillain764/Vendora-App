using Application.DTO.UserDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId);
        Task<Result<UserResponseForOthersDTO>> GetOtherAsync(Ulid UserId);
        Task<Result<UserResponseForItselfDTO>> ChangeUserProfileNameAsync(Ulid UserId, UserChangeProfileNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserFirstNameAsync(Ulid UserId, UserChangeFirstNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLastNameAsync(Ulid UserId, UserChangeLastNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLoginAsync(Ulid UserId, UserChangeLastNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserPasswordAsync(Ulid UserId, UserChangePasswordDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserEmailAsync(Ulid UserId, UserChangeEmailDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserPhoneAsync(Ulid UserId, UserChangePhoneDTO DTO);
        // later add change avatar
        Task<Result<UserResponseForItselfDTO>> ChangeUserGenderAsync(Ulid UserId, UserChangePhoneDTO DTO);
    }
}