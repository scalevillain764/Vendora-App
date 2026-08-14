using Application.DTO.StoreDTO;
using Application.DTO.UserDTO;
using Application.Interfaces;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Stores;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using IStoreService = Application.Interfaces.IStoreService;
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

        public async Task<Result<string>> RemoveMyStoreAsync(Ulid UserId)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            if (store == null)
                return Result<string>.Error("Магазин не найден", ErrorType.NotFound);

            store.IsDeleted = true;

            await _context.SaveChangesAsync();
            return Result<string>.Success("OK");

        }

        private async Task<Result<StoreOwnerResponseDTO>> ChangeStorePropertyAsync(Ulid UserId, Action<Store> action)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            if (store == null)
                return Result<StoreOwnerResponseDTO>.Error("Магази не создан", ErrorType.Forbidden);

            action(store);

            await _context.SaveChangesAsync();

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store, true));
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

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(newStore, true));
        }

        public async Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            return store != null
                ? Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store, true))
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
        
        // pics
        public async Task<Result<StoreOwnerResponseDTO>> ChangeStoreAvatarAsync(Ulid UserId, IFormFile? file)
        {
            var store = await _context.Stores
                .FirstOrDefaultAsync(x => x.SellerId == UserId);

            if (store == null)
                return Result<StoreOwnerResponseDTO>.Error("Магазин не найден", ErrorType.NotFound);

            string? old_url = store.UrlAvatar;
            string? new_url = null;

            if (file == null)
            {
                if (store.UrlAvatar != null)
                    store.UrlAvatar = null;
            }    
            else
            {
                var loadPicture = await _S3Service.UploadPhotoAsync(file);
                if (!loadPicture.IsSuccess)
                    return Result<StoreOwnerResponseDTO>.Error(loadPicture.ErrorMessage, loadPicture.ErrorType ?? ErrorType.Conflict);
                new_url = loadPicture.data;
                store.UrlAvatar = new_url;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (new_url != null)
                    await _S3Service.RemovePhotoByUrlAsync(new_url);
                throw;
            }

            await _S3Service.RemovePhotoByUrlAsync(old_url);
            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store, true));
        }

        public Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid UserId, StoreChangeDescriptionDTO DTO)
           => ChangeStorePropertyAsync(UserId, x => x.Description = DTO.Description);
    }
}