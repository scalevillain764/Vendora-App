namespace Application.DTO.ProductDTO.StoreDTO
{
    public record ProductCreationDTO(
         string StoreId,
         int Category,
         string Name,
         string? Description,
         string? ShortDescription,
         decimal Price,
         int Quantity,
         string? PreviewUrl
    );
}