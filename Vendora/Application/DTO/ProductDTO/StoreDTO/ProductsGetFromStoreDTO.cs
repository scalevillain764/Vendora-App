namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductsGetFromStoreDTO(Ulid StoreId, int page, int pageSize);
}