using Application.DTO.UserDTO;
using Application.Result;
using IUserService = Application.Interfaces.IUserService;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Domain.ErrorTypes;
using Domain.Users;
namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        private async Task<Result<UserResponseForItselfDTO>> ChangeUserPropertyAsync(Ulid UserId, Action<User> action) 
        {
            var user = await _context.Users
                .FindAsync(UserId);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            action(user);

            await _context.SaveChangesAsync();

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user));
        }

        public async Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId) // just get main data about user
        {
            var user = await _context.Users
                .FindAsync(UserId);

            return user != null ? Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user))
                : Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);
        }

        public async Task<Result<UserResponseForOthersDTO>> GetOtherAsync(Ulid UserId)
        {
            var user = await _context.Users
                .FindAsync(UserId);

            return user != null ? Result<UserResponseForOthersDTO>.Success(new UserResponseForOthersDTO(user))
                : Result<UserResponseForOthersDTO>.Error("Пользователь не найден", ErrorType.NotFound);
        }

        public Task<Result<UserResponseForItselfDTO>> ChangeUserProfileNameAsync(Ulid UserId, UserChangeProfileNameDTO DTO)
            => ChangeUserPropertyAsync(UserId, u => u.ProfileName = DTO.ProfileName);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserFirstNameAsync(Ulid UserId, UserChangeFirstNameDTO DTO)
           =>  ChangeUserPropertyAsync(UserId, u => u.FirstName = DTO.FirstName);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserLastNameAsync(Ulid UserId, UserChangeLastNameDTO DTO)
            => ChangeUserPropertyAsync(UserId, u => u.LastName = DTO.LastName);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserEmailAsync(Ulid UserId, UserChangeEmailDTO DTO)
            => ChangeUserPropertyAsync(UserId, u => u.Email = DTO.Email);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserPhoneAsync(Ulid UserId, UserChangePhoneDTO DTO)
           => ChangeUserPropertyAsync(UserId, u => u.Phone = DTO.Phone);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserGenderAsync(Ulid UserId, UserChangeGenderDTO DTO)
            => ChangeUserPropertyAsync(UserId, u => u.UserGender = (User.Gender)DTO.Gender);       
    } 
}