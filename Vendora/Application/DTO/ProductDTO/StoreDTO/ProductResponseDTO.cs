using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductResponseDTO(
         string StoreId,
         int CategoryId,
         string Name,
         string? Description,
         decimal Price,
         int Quantity,
         string? PreviewUrl,
         long Article 
    )
    {
        public ProductResponseDTO(Product pr) : 
            this(pr.StoreId.ToString(), pr.CategoryId, pr.Name, pr.Description, pr.Price, pr.Quantity, pr.PreviewUrl, pr.Article)
        { }
    }
}