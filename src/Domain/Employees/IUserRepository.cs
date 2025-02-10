using Microsoft.AspNetCore.Identity;

namespace Domain.Employees;

public interface IUserRepository
{
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByDocumentAsync(string documentNumber);
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<string?> GetUserRoleAsync(ApplicationUser user);
    Task<IEnumerable<ApplicationUser>> GetUsersAsync(EmployeeFilter employeeFilter);
    Task<List<UserPhone>> GetUserPhonesAsync(string id);
}
