using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCardDTO(
        string StoreName,
        string Name,
        decimal Price,
        string? PreviewUrl,
        bool IsFavourite
    )
    {
        public ProductCardDTO(Product product, bool isFavourite) : 
            this(product.Store.Name, product.Name, product.Price, product.PreviewUrl, isFavourite) { }
    }
}