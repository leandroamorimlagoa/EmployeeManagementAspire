
namespace Domain.Authentications;

public interface IAuthenticationService
{
    Task<string> Authenticate(string email, string password);
}
