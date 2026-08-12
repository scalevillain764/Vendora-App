using Amazon.S3;
using Amazon.S3.Model;
using Application.DTO.ProductDTO.StoreDTO;
using Application.DTO.UserDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Products;
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

            int ordersMade = await _context.Orders
                .CountAsync(x => x.UserId == UserId);

            int reviewsLeft = await _context.ProductReviews
                .CountAsync(x => x.UserId == UserId);

            await _context.SaveChangesAsync();

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user, ordersMade, reviewsLeft));
        }
        public async Task<Result<string>> DeleteMyAccountAsync(Ulid UserId)
        {
            var user = await _context.Users
                .Include(x => x.Store)
                .FirstOrDefaultAsync(x => x.Id == UserId);

            if (user == null)
                return Result<string>.Error("Что-то пошло не так", ErrorType.NotFound);

            if (user.Store != null)
                user.Store.IsDeleted = true;

            user.IsDeleted = true;

            await _context.SaveChangesAsync();

            return Result<string>.Success("OK");
        }
        public async Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId) // just get main data about user
        {
            var user = await _context.Users
                .FindAsync(UserId);

            int ordersMade = await _context.Orders
                .CountAsync(x => x.UserId == UserId);

            int reviewsLeft = await _context.ProductReviews
                .CountAsync(x => x.UserId == UserId);

            return user != null ? Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user, ordersMade, reviewsLeft))
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
            var loadPicture = await _S3Service.UploadPhotoAsync(file);
            if (!loadPicture.IsSuccess)
                return Result<UserResponseForItselfDTO>.Error(loadPicture.ErrorMessage, 
                    loadPicture.ErrorType ?? ErrorType.Conflict);

            string NewUrl = loadPicture.data;
            string? PrevUrl = null;

            var updateUserProfilePictureUrl = await ChangeUserPropertyAsync(UserId, x =>
            {
                PrevUrl = x.AvatarUrl;
                x.AvatarUrl = NewUrl;
            });

            if (!updateUserProfilePictureUrl.IsSuccess)
            {
                await _S3Service.RemovePhotoByUrlAsync(NewUrl);
                return Result<UserResponseForItselfDTO>.Error(updateUserProfilePictureUrl.ErrorMessage,
                    updateUserProfilePictureUrl.ErrorType ?? ErrorType.Conflict);
            }

            if (!string.IsNullOrEmpty(PrevUrl))
                await _S3Service.RemovePhotoByUrlAsync(PrevUrl);

            return Result<UserResponseForItselfDTO>.Success(updateUserProfilePictureUrl.data);
        }

        public async Task<Result<UserResponseForItselfDTO>> RemoveProfilePictureAsync(Ulid UserId, UserRemoveProfilePictureDTO DTO)
        {           
            var updateUserProfilePicture = await ChangeUserPropertyAsync(UserId, x => x.AvatarUrl = null);

            if (!updateUserProfilePicture.IsSuccess)
                return Result<UserResponseForItselfDTO>.Error(updateUserProfilePicture.ErrorMessage,
              updateUserProfilePicture.ErrorType ?? ErrorType.Conflict);

            var removeUserPictureFromS3 = await _S3Service.RemovePhotoByUrlAsync(DTO.fileURL);

            if (!removeUserPictureFromS3.IsSuccess)
            {
                await ChangeUserPropertyAsync(UserId, x => x.AvatarUrl = DTO.fileURL);
                return Result<UserResponseForItselfDTO>.Error(removeUserPictureFromS3.ErrorMessage,
                    removeUserPictureFromS3.ErrorType ?? ErrorType.Conflict);
            }

            return Result<UserResponseForItselfDTO>.Success(updateUserProfilePicture.data);
        }
       // pics
    } 
}