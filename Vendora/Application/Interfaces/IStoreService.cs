using Application.DTO.StoreDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IStoreService
    {
        Task<Result<StoreOwnerResponseDTO>> CreateStoreAsync(Ulid UserId, StoreOwnerCreateDTO dto, CancellationToken token);
        Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId, CancellationToken token);
        Task<Result<string>> RemoveMyStoreAsync(Ulid UserId, CancellationToken token);
        Task<Result<StorePublicResponseDTO>>GetStoreAsync(Ulid StoreId, CancellationToken token);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreNameAsync(Ulid UserId, StoreChangeNameDTO DTO, CancellationToken token);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreAvatarAsync(Ulid UserId, IFormFile? file, CancellationToken token);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid UserId, StoreChangeDescriptionDTO DTO, CancellationToken token);
    }
}
