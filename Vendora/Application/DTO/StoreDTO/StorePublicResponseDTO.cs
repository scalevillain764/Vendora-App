using Domain.Stores;
namespace Application.DTO.StoreDTO
{
    public record StorePublicResponseDTO(
        Ulid StoreId,
        string Name,
        string? Description,
        string? UrlAvatar
    )
    {
        public StorePublicResponseDTO(Store store)
            : this(store.Id, store.Name, store.Description, store.UrlAvatar) { }
    }
}