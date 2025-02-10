using System.Security.Claims;
using Domain.Core;
using Domain.Employees;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters;

public class AuthenticationFilter : IAsyncActionFilter
{
    private readonly ILogger<AuthenticationFilter> _logger;
    private readonly AuthenticatedUser authenticatedUser;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationFilter(ILogger<AuthenticationFilter> logger,
                                AuthenticatedUser authenticatedUser,
                                UserManager<ApplicationUser> userManager,
                                IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        this.authenticatedUser = authenticatedUser;
        this.userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        if (user.Identity.IsAuthenticated)
        {
            authenticatedUser.Id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            authenticatedUser.Email = user.FindFirstValue(ClaimTypes.Email);
            authenticatedUser.FirstName = user.FindFirstValue(ClaimTypes.GivenName);
            authenticatedUser.LastName = user.FindFirstValue(ClaimTypes.Surname);
            authenticatedUser.Role = await GetRoleFromUser(authenticatedUser.Id);
        }

        await next();
    }

    private async Task<string?> GetRoleFromUser(string? id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        var user = await userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }
}
