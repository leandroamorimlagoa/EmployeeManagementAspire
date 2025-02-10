using Application.Models.Employees;
using Domain.Employees;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IUserService userService;

    public EmployeeController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    public Task<IEnumerable<EmployeeModel>> Get([FromQuery] EmployeeModelFilter filter)
    => userService.GetUsersAsync(filter);

    [HttpGet("{id}")]
    public Task<EmployeeModel?> Get(string id)
    => userService.GetUserByIdAsync(id);

    [HttpPut]
    public Task Update([FromBody] RegisterModel model)
    => userService.UpdateUserAsync(model);

    [HttpPost]
    public Task Register([FromBody] RegisterModel model)
    => userService.RegisterUserAsync(model);

    [HttpDelete("{id}")]
    public Task Delete(string id)
    => userService.DeleteUserAsync(id);
}
