namespace Application.DTO.StoreDTO
{
    public record StoreOwnerResponseDTO(
        string Name,
        string? Description,
        string? UrlAvatar,
        int ProductCounter
    );
}