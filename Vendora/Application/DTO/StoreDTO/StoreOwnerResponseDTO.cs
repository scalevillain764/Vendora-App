using Domain.Stores;
namespace Application.DTO.StoreDTO
{
    public record StoreOwnerResponseDTO(
        string Name,
        string? Description,
        string? UrlAvatar,
        int ProductCounter // for difference now, then update
    )
    {
        public StoreOwnerResponseDTO(Store store)
            : this(store.Name, store.Description, store.UrlAvatar, store.Products.Count) { }
    }
}