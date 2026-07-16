using Domain.Stores;
namespace Application.DTO.StoreDTO
{
    public record StorePublicResponseDTO(
        string Name,
        string? Description,
        string? UrlAvatar
    )
    {
        public StorePublicResponseDTO(Store store)
            : this (store.Name, store.Description, store.UrlAvatar) { }
    }
}