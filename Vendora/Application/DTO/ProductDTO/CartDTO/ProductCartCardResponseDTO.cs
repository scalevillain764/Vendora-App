using Domain.Products;
namespace Application.DTO.ProductDTO.CartDTO
{
    public record ProductCartCardResponseDTO(
        Ulid ProductId,
        string Name,
        decimal PricePerUnit,
        string? ShortDescription,
        string? PreviewUrl,
        decimal PricePerUnit,
        int Quantity)
    {
        public ProductCartCardResponseDTO(Product product, int quantity)
            : this(product.Id, product.Name, product.Price, product.ShortDescription, product.PreviewUrl, quantity) { }
    }
}