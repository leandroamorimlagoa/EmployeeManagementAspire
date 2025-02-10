using Application.Models.Employees;
using Domain.Employees;
using Riok.Mapperly.Abstractions;

namespace Application.Services.Employees;

[Mapper]
public static partial class EmployeeMappings
{
    public static partial EmployeeModel MapToEmployeeModel(this ApplicationUser obj);

    public static partial RegisterPhoneModel MapToEmployeePhoneModel(this UserPhone obj);

    public static partial IEnumerable<RegisterPhoneModel> MapToEmployeePhonesModel(this IEnumerable<UserPhone> list);

    public static partial ICollection<UserPhone> MapToEmployeePhones(this IEnumerable<RegisterPhoneModel> list);

    //public static partial EmployeeModel MapToEmployeeModel(this Employee obj);

    //public static partial EmployeeModelFilter MapToEmployeeModelFilter(this EmployeeFilter? obj);

    public static partial EmployeeFilter MapToEmployeeFilter(this EmployeeModelFilter? obj);

    public static IEnumerable<EmployeeModel> MapToEmployeeModelList(this IEnumerable<ApplicationUser> query)
    {
        return query.Select(c => c.MapToEmployeeModel()).ToList();
    }
}
