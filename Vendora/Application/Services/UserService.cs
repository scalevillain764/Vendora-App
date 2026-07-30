using Amazon.S3;
using Amazon.S3.Model;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using Yandex.Checkout.V3;
using IS3Service = Application.Interfaces.IS3Service;
using IUserService = Application.Interfaces.IUserService;
namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IS3Service _S3Service;
        public UserService(AppDbContext context, IS3Service S3Service)
        {
            _context = context;
            _S3Service = S3Service;
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

        public async Task<Result<UserResponseForOthersDTO>> GetUserAsync(Ulid UserId)
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
        
        // pictures
        public async Task<Result<UserResponseForItselfDTO>> ChangeUserProfilePictureAsync(Ulid UserId, IFormFile file)
        {
            var fileUrlResult = await _S3Service.UploadPhotoAsync(file);

            if (!fileUrlResult.IsSuccess)
                return Result<UserResponseForItselfDTO>.Error(fileUrlResult.ErrorMessage, fileUrlResult.ErrorType ?? ErrorType.Validation);

            string fileUrl = fileUrlResult.data;

            var user = await _context.Users
                .FindAsync(UserId);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);
                 
            user.AvatarUrl = fileUrl;
            await _context.SaveChangesAsync();

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user));
        }

        public async Task<Result<UserResponseForItselfDTO>> RemoveProfilePictureAsync(Ulid UserId, UserRemoveProfilePictureDTO DTO)
        {
            var user = await _context.Users
                .FindAsync(UserId);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            if (user.AvatarUrl != DTO.fileURL)
                return Result<UserResponseForItselfDTO>.Error("Проверьте корректность данных", ErrorType.Conflict);

            var str_rez = await _S3Service.RemovePhotoByUrlAsync(DTO.fileURL);

            if (!str_rez.IsSuccess)
                return Result<UserResponseForItselfDTO>.Error(str_rez.ErrorMessage, str_rez.ErrorType ?? ErrorType.Conflict);

            user.AvatarUrl = null;
            await _context.SaveChangesAsync();

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user));
        }
    } 
}