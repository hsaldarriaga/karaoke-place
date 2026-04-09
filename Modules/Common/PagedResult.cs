namespace karaoke_place.Modules.Common;

public class PagedResult<T>
{
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public required IEnumerable<T> Items { get; set; } = [];
}
