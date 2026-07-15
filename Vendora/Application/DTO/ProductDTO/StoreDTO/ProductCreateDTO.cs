namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCreationDTO(
         string StoreId,
         int CategoryId,
         string Name,
         string? Description,
         decimal Price,
         int Quantity,
         string? PreviewUrl
    );
}