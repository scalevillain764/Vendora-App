using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCardDTO (
        string StoreName,   
        string Name,
        decimal Price,
        string? PreviewUrl
    )
    {
        public ProductCardDTO(Product product) : 
            this(product.Store.Name, product.Name, product.Price, product.PreviewUrl) { }
    }
}