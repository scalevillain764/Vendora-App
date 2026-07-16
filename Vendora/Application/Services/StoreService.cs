using Application.DTO.StoreDTO;
using Application.Result;
using Domain.ErrorTypes;
using Domain.Users;
using Infrastructure.AppDbContexts;
using Microsoft.EntityFrameworkCore;
using IStoreService = Application.Interfaces.IStoreService;
using Domain.Stores;
namespace Application.Services
{
    public class StoreService : IStoreService
    {
        private readonly AppDbContext _context;
        public StoreService(AppDbContext context)
        {
            _context = context;
        }
        private async Task<Result<StoreOwnerResponseDTO>> ChangeStorePropertyAsync(Ulid StoreId, Action<Store> action)
        {
            var store = await _context.Stores
                .FindAsync(StoreId);

            if (store == null)
                return Result<StoreOwnerResponseDTO>.Error("Магази не создан", ErrorType.Forbidden);

            action(store);

            await _context.SaveChangesAsync();

            return Result<StoreOwnerResponseDTO>.Success(new StoreOwnerResponseDTO(store));
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
                 .FindAsync(StoreId);

            return store != null
                ? Result<StorePublicResponseDTO>.Success(new StorePublicResponseDTO(store))
                : Result<StorePublicResponseDTO>.Error("Магазин не найдкен", ErrorType.NotFound);
        }
        public Task<Result<StoreOwnerResponseDTO>> ChangeStoreNameAsync(Ulid UserId, StoreChangeNameDTO DTO) 
            => ChangeStorePropertyAsync(UserId, x => x.Name = DTO.Name);

        public Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid UserId, StoreChangeDescriptionDTO DTO)
           => ChangeStorePropertyAsync(UserId, x => x.Description = DTO.Description);
    }
}