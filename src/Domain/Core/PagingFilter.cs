namespace Domain.Core;

public class PagingFilter
{
    public int PageSize { get; set; } = 20;
    public int CurrentPage { get; set; } = 1;
}
