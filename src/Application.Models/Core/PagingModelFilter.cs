namespace Application.Models.Core;

public class PagingModelFilter
{
    public int PageSize { get; set; } = 20;
    public int CurrentPage { get; set; } = 1;
}
