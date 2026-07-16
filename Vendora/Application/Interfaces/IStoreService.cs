using Application.DTO.StoreDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IStoreService
    {
        Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId);
        Task<Result<StorePublicResponseDTO>> GetStoreAsync(Ulid StoreId);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreNameAsync(Ulid StoreId, StoreChangeNameDTO DTO);
        Task<Result<StoreOwnerResponseDTO>> ChangeStoreDescriptionAsync(Ulid StoreId, StoreChangeDescriptionDTO DTO);
    }
}