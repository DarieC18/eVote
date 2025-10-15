namespace EVote360.Application.Results;

public sealed class PagedResult<T> : Result<IEnumerable<T>>
{
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int Total { get; private set; }

    public static PagedResult<T> Success(IEnumerable<T> data, int page, int pageSize, int total)
        => new()
        {
            Succeeded = true,
            Data = data,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
}
