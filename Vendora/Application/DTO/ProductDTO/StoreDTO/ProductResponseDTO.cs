using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductResponseDTO(
         string StoreId,
         string Category,
         string Name,
         string? Description,
         decimal Price,
         int Quantity,
         string? PreviewUrl,
         long Article 
    )
    {
        public ProductResponseDTO(Product pr) : 
            this(pr.StoreId.ToString(), pr.Category.ToString(), pr.Name, pr.Description, pr.Price, pr.Quantity, pr.PreviewUrl, pr.Article)
        { }
    }
}