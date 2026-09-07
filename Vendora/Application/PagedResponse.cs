namespace Application.PagedResponse
{
    public record PagedResponse<T>(
        IEnumerable<T> items,
        int pages,
        int pageSize,
        int totalCount
        );
}