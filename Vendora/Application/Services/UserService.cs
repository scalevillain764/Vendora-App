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
using Newtonsoft.Json.Linq;
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
        private async Task<Result<UserResponseForItselfDTO>> ChangeUserPropertyAsync(Ulid UserId, Action<User> action, CancellationToken token) 
        {
            var user = await _context.Users
                .FindAsync(UserId, token);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            action(user);

            int ordersMade = await _context.Orders
                .CountAsync(x => x.UserId == UserId, token);

            int reviewsLeft = await _context.ProductReviews
                .CountAsync(x => x.UserId == UserId, token);

            await _context.SaveChangesAsync(token);

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user, ordersMade, reviewsLeft));
        }
        public async Task<Result<string>> DeleteMyAccountAsync(Ulid UserId, CancellationToken token)
        {
            var user = await _context.Users
                .Include(x => x.Store)
                .FirstOrDefaultAsync(x => x.Id == UserId, token);

            if (user == null)
                return Result<string>.Error("Что-то пошло не так", ErrorType.NotFound);

            if (user.Store != null)
                user.Store.IsDeleted = true;

            user.IsDeleted = true;

            await _context.SaveChangesAsync(token);

            return Result<string>.Success("OK");
        }
        public async Task<Result<UserResponseForItselfDTO>> GetMeAsync(Ulid UserId, CancellationToken token) // just get main data about user
        {
            var user = await _context.Users
                .FindAsync(UserId, token);

            int ordersMade = await _context.Orders
                .CountAsync(x => x.UserId == UserId, token);

            int reviewsLeft = await _context.ProductReviews
                .CountAsync(x => x.UserId == UserId, token);

            return user != null ? Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user, ordersMade, reviewsLeft))
                : Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);
        }

        public async Task<Result<UserResponseForOthersDTO>> GetUserAsync(Ulid UserId, CancellationToken token)
        {
            var user = await _context.Users
                .FindAsync(UserId, token);

            return user != null ? Result<UserResponseForOthersDTO>.Success(new UserResponseForOthersDTO(user))
                : Result<UserResponseForOthersDTO>.Error("Пользователь не найден", ErrorType.NotFound);
        }

        public Task<Result<UserResponseForItselfDTO>> ChangeUserProfileNameAsync(Ulid UserId, UserChangeProfileNameDTO DTO, CancellationToken token)
            => ChangeUserPropertyAsync(UserId, u => u.ProfileName = DTO.ProfileName, token);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserFirstNameAsync(Ulid UserId, UserChangeFirstNameDTO DTO, CancellationToken token)
           =>  ChangeUserPropertyAsync(UserId, u => u.FirstName = DTO.FirstName, token);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserLastNameAsync(Ulid UserId, UserChangeLastNameDTO DTO, CancellationToken token)
            => ChangeUserPropertyAsync(UserId, u => u.LastName = DTO.LastName, token);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserEmailAsync(Ulid UserId, UserChangeEmailDTO DTO, CancellationToken token)
            => ChangeUserPropertyAsync(UserId, u => u.Email = DTO.Email, token);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserPhoneAsync(Ulid UserId, UserChangePhoneDTO DTO, CancellationToken token)
           => ChangeUserPropertyAsync(UserId, u => u.Phone = DTO.Phone, token);

        public Task<Result<UserResponseForItselfDTO>> ChangeUserGenderAsync(Ulid UserId, UserChangeGenderDTO DTO, CancellationToken token)
            => ChangeUserPropertyAsync(UserId, u => u.UserGender = (User.Gender)DTO.Gender, token);

        // pictures
        public async Task<Result<UserResponseForItselfDTO>> ChangeUserProfilePictureAsync(Ulid UserId, IFormFile? file, CancellationToken token)
        {
            var user = await _context.Users
                .FindAsync(UserId, token);

            if (user == null)
                return Result<UserResponseForItselfDTO>.Error("Пользователь не найден", ErrorType.NotFound);

            string? oldUrl = user.AvatarUrl;
            string? newUrl = null;

            if(file == null)
            {
                if (user.AvatarUrl != null)
                    user.AvatarUrl = null;
            }
            else
            {
                var loadFile = await _S3Service.UploadPhotoAsync(file);
                if (!loadFile.IsSuccess)
                    return Result<UserResponseForItselfDTO>.Error(loadFile.ErrorMessage,
                        loadFile.ErrorType ?? ErrorType.Conflict);
                newUrl = loadFile.data;
            }

            try
            {
                await _context.SaveChangesAsync(token);
            } 
            catch
            {
                if (newUrl != null)
                    await _S3Service.RemovePhotoByUrlAsync(newUrl);
                throw;
            }

            if (oldUrl != null)
                await _S3Service.RemovePhotoByUrlAsync(oldUrl);

            int ordersMade = await _context.Orders
                .CountAsync(x => x.UserId == UserId, token);

            int reviewsLeft = await _context.ProductReviews
                .CountAsync(x => x.UserId == UserId, token);

            return Result<UserResponseForItselfDTO>.Success(new UserResponseForItselfDTO(user, ordersMade, reviewsLeft));
        }      
       // pics
    } 
}