
 using Application.DTO.StoreDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IStoreService = Application.Interfaces.IStoreService;
using Domain.Stores;
using Application.Interfaces;
using System.Runtime.Intrinsics.X86;
namespace Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly AppDbContext _context;
        private readonly IS3Service _S3Service;
        public StoreService(AppDbContext context, IS3Service S3Service)
        {
            _context = context;
            _S3Service = S3Service;
        }
        private async Task<Result<StoreOwnerResponseDTO>> ChangeStorePropertyAsync(Ulid UserId, Action<Store> action)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            if (store == null)
                return Result<StoreOwnerResponseDTO>.Error("Магази не создан", ErrorType.Forbidden);

            action(store);

            await _context.SaveChangesAsync();

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store));
        }

        public async Task<Result<StoreOwnerResponseDTO>> CreateStoreAsync(Ulid UserId, StoreOwnerCreateDTO dto)
        {
            bool storeExists = await _context.Stores
                .AnyAsync(x => x.SellerId == UserId);

            if (storeExists)
                return Result<StoreOwnerResponseDTO>.Error("У вас уже существует магазин", ErrorType.Forbidden);

            var newStore = new Store(UserId, dto.Name, dto.Description, dto.UrlAvatar);

            _context.Stores.Add(newStore);

            await _context.SaveChangesAsync();

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(newStore));
        }

        public async Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            return store != null
                ? Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store))
                : Result<StoreOwnerResponseDTO>.Error("Магазин не создан", ErrorType.Forbidden);              
        }

        public async Task<Result<StorePublicResponseDTO>> GetStoreAsync(Ulid StoreId)
        {
            var store = await _context.Stores
                 .FirstOrDefaultAsync(x => x.Id == StoreId);

            return store != null
                ? Result<StorePublicResponseDTO>.Success(new StorePublicResponseDTO(store))
                : Result<StorePublicResponseDTO>.Error("Магазин не найдкен", ErrorType.NotFound);
        }

        public Task<Result<StoreOwnerResponseDTO>> ChangeStoreNameAsync(Ulid UserId, StoreChangeNameDTO DTO) 
            => ChangeStorePropertyAsync(UserId, x => x.Name = DTO.Name);

        public async Task<Result<StoreOwnerResponseDTO>> ChangeStoreAvatarAsync(Ulid UserId, IFormFile file)
        {
            var store = await _context.Stores
             .FirstOrDefaultAsync(x => x.SellerId == UserId);

            if (store == null)
                return Result<StoreOwnerResponseDTO>.Error("Магази не создан", ErrorType.Forbidden);

            var loadPhotoResult = await _S3Service.UploadPhotoAsync(file);
            if (!loadPhotoResult.IsSuccess)
                return Result<StoreOwnerResponseDTO>.Error(loadPhotoResult.ErrorMessage, loadPhotoResult.ErrorType ?? ErrorType.Conflict);

            string url = loadPhotoResult.data;

            store.UrlAvatar = url;

            await _context.SaveChangesAsync();

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store));
        }

        public async Task<Result<StoreOwnerResponseDTO>> RemoveStoreAvatarAsync(Ulid UserId, StoreRemoveAvatarUrlDTO DTO)
        {
            var updateStoreAvatarUrl = await ChangeStorePropertyAsync(UserId, x => x.UrlAvatar = null);

            if(!updateStoreAvatarUrl.IsSuccess)
                return Result<StoreOwnerResponseDTO>.Error(updateStoreAvatarUrl.ErrorMessage, updateStoreAvatarUrl.ErrorType ?? ErrorType.Conflict);

            var removePhotoFromS3Result = await _S3Service.RemovePhotoByUrlAsync(DTO.AvatarUrl);

            if (!removePhotoFromS3Result.IsSuccess)
            {
                await ChangeStorePropertyAsync(UserId, x => x.UrlAvatar = DTO.AvatarUrl);
                return Result<StoreOwnerResponseDTO>.Error(removePhotoFromS3Result.ErrorMessage, removePhotoFromS3Result.ErrorType ?? ErrorType.Conflict);
            }

            return Result<StoreOwnerResponseDTO>.Success(updateStoreAvatarUrl.data);
        }

        public Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid UserId, StoreChangeDescriptionDTO DTO)
           => ChangeStorePropertyAsync(UserId, x => x.Description = DTO.Description);
    }
}