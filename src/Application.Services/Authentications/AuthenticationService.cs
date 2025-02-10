using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Authentications;
using Domain.Employees;
using Domain.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Authentications;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly NotificationContext _notificationContext;
    private readonly IConfiguration _configuration;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    NotificationContext notificationContext,
                                    IConfiguration configuration)
    {
        this._userManager = userManager;
        this._signInManager = signInManager;
        this._notificationContext = notificationContext;
        this._configuration = configuration;
    }

    public async Task<string> Authenticate(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            _notificationContext.AddNotification("Invalid credentials");
            return string.Empty;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var value = _configuration.GetSection("Jwt:Key").Value ?? "CHAVE_SECRETA_SUPER_SECRETA_LONGA_E_SEGURA_32_CHARSAKI";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(value));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
