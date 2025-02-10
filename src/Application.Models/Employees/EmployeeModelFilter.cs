using Application.Models.Core;

namespace Application.Models.Employees;

public class EmployeeModelFilter : PagingModelFilter
{
    public string? SearchTerm { get; set; }
}
