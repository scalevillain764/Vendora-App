using Application.DTO.StoreDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IStoreService
    {
        Task<Result<StoreOwnerResponseDTO>> CreateStoreAsync(Ulid UserId, StoreOwnerCreateDTO dto);
        Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId);
        Task<Result<string>> RemoveMyStoreAsync(Ulid UserId);
        Task<Result<StorePublicResponseDTO>> GetStoreAsync(Ulid StoreId);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreNameAsync(Ulid UserId, StoreChangeNameDTO DTO);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreAvatarAsync(Ulid UserId, IFormFile? file);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid UserId, StoreChangeDescriptionDTO DTO);
    }
}
