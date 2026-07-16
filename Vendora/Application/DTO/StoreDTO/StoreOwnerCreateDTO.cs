namespace Application.DTO.StoreDTO
{
    public record StoreOwnerCreateDTO(
        string Name,
        string? Description,
        string? UrlAvatar
    );
}