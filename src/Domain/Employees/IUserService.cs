using Application.Models.Employees;

namespace Domain.Employees;

public interface IUserService
{
    Task RegisterUserAsync(RegisterModel model);
    Task UpdateUserAsync(RegisterModel model);
    Task DeleteUserAsync(string id);
    Task<EmployeeModel?> GetUserByIdAsync(string id);
    Task<IEnumerable<EmployeeModel>> GetUsersAsync(EmployeeModelFilter filterModel);
}
