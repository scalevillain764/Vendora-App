using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCardDTO(
        Ulid ProductId,
        string StoreName,
        string Name,
        decimal Price,
        string? ShortDescription,
        string? PreviewUrl,
        bool IsFavourite
    )
    {
        public ProductCardDTO(Product product, bool isFavourite) : 
            this(product.Id, product.Store.Name, product.Name, product.Price, product.ShortDescription, product.PreviewUrl, isFavourite) { }
    }
}