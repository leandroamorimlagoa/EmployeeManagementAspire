using Application.Models.Authentications;
using Domain.Authentications;
using Domain.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly AuthenticatedUser authenticatedUser;

    public AuthenticateController(IAuthenticationService authenticationService,
                                    AuthenticatedUser authenticatedUser)
    {
        this.authenticationService = authenticationService;
        this.authenticatedUser = authenticatedUser;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Authenticate([FromBody] LoginModel model)
    {
        var token = await this.authenticationService.Authenticate(model.Email, model.Password);
        if (string.IsNullOrEmpty(token)) return Unauthorized();
        return Ok(new { token });
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetCurrentUser()
    {
        return Ok(this.authenticatedUser);
    }
}
