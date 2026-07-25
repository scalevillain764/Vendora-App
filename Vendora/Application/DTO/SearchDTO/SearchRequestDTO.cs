namespace Application.DTO.SearchDTO
{
    public record SearchRequestDTO(
        string? Query,
        List<int>? CategoryIds,
        decimal? MinPrice,
        decimal? MaxPrice,
        bool? OnlyInStock
    );
}