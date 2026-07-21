namespace Application.DTO.FavouriteDTO
{
    public record FavoriteResponseDTO(
        Ulid ProductId,
        bool IsFavorite
    );
}