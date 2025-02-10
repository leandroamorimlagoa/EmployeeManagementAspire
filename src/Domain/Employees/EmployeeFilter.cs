using Domain.Core;

namespace Domain.Employees;

public class EmployeeFilter : PagingFilter
{
    public string? SearchTerm { get; set; }
}
