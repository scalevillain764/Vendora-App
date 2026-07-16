using Application.DTO.UserDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId);
        Task<Result<UserResponseForOthersDTO>> GetUserAsync(Ulid UserId);
        Task<Result<UserResponseForItselfDTO>> ChangeUserProfileNameAsync(Ulid UserId, UserChangeProfileNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserFirstNameAsync(Ulid UserId, UserChangeFirstNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserLastNameAsync(Ulid UserId, UserChangeLastNameDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserEmailAsync(Ulid UserId, UserChangeEmailDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserPhoneAsync(Ulid UserId, UserChangePhoneDTO DTO);
        Task<Result<UserResponseForItselfDTO>> ChangeUserGenderAsync(Ulid UserId, UserChangeGenderDTO DTO);
        // later add change avatar
    }
}