using Application.DTO.StoreDTO;
using Application.Result;
namespace Application.Interfaces
{
    public interface IStoreService
    {
        Task<Result<StoreOwnerResponseDTO>> GetMyStoreAsync(Ulid UserId);
        Task<Result<StorePublicResponseDTO>> GetOtherStore(Ulid StoerId);
        
    }
}