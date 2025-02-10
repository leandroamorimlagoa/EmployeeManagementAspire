

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Employees;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient WithAuthenticatedUser(this HttpClient client, ApplicationUser user)
    {
        var token = GenerateToken(user);
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Remove("Authorization");  // Remove caso já tenha um token anterior
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        return client;
    }

    private static string? GenerateToken(ApplicationUser user)
    {
        var audience = "https://{--app-audience--}";
        var issuer = "https://{--app-issuer--}";
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVE_SECRETA_SUPER_SECRETA_LONGA_E_SEGURA_32_CHARSAKI")),
                SecurityAlgorithms.HmacSha256
            )
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHAVE_SECRETA_SUPER_SECRETA_LONGA_E_SEGURA_32_CHARSAKI"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenObject = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(tokenObject);
    }
}