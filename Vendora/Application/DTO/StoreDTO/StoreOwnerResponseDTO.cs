using Domain.Stores;
namespace Application.DTO.StoreDTO
{
    public record StoreOwnerResponseDTO(
        Ulid StoreId,
        string Name,
        string? Description,
        string? UrlAvatar,
        int ProductCounter, // for difference now, then update
        bool IsOwner
    )
    {
        public StoreOwnerResponseDTO(Store store, bool IsOwner)
            : this(store.Id, store.Name, store.Description, store.UrlAvatar, store.Products.Count, IsOwner) { }
    }
}