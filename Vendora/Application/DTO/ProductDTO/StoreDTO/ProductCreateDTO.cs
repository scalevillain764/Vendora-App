using Domain.Products;
namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCreationDTO(
         string StoreId,
         Product.ProductCategory Category,
         string Name,
         string? Description,
         string? ShortDescription,
         decimal Price,
         int Quantity,
         string? PreviewUrl,
         List<string>? Pictures
    );
}