using Domain.Employees;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Employees;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetUserByDocumentAsync(string documentNumber)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.DocNumber == documentNumber);
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password, string role)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return result;
        }
        var test = await _userManager.FindByEmailAsync(user.Email);
        result = await _userManager.AddToRoleAsync(user, role);
        return result;
    }

    public Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
    => _userManager.UpdateAsync(user);

    public Task<ApplicationUser?> GetUserByIdAsync(string id)
    => _userManager.FindByIdAsync(id);

    public async Task<string?> GetUserRoleAsync(ApplicationUser user)
    => (await _userManager.GetRolesAsync(user)).FirstOrDefault();

    public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(EmployeeFilter employeeFilter)
    => await _userManager.Users.Where(u => string.IsNullOrEmpty(employeeFilter.SearchTerm) ||
                                                            (u.FirstName.Contains(employeeFilter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                             u.LastName.Contains(employeeFilter.SearchTerm, StringComparison.OrdinalIgnoreCase)))
                                .OrderBy(u=>u.FirstName)
                                .Skip((employeeFilter.CurrentPage - 1) * employeeFilter.PageSize)
                                .Take(employeeFilter.PageSize)
                                .ToListAsync();

    public Task<List<UserPhone>> GetUserPhonesAsync(string id)
    => _userManager.Users.Include(u => u.Phones).Where(u => u.Id == id).SelectMany(u => u.Phones).ToListAsync();
}
