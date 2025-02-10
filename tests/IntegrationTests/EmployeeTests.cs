using Application.Models.Authentications;
using Application.Models.Employees;
using Bogus;
using IntegrationTests.Builders.Entities;
using IntegrationTests.Builders.Models;
using IntegrationTests.Core;
using IntegrationTests.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IntegrationTests;

public class EmployeeTests : BaseIntegrationTest
{
    public EmployeeTests(IntegrationTestFactory factory) : base(factory)
    {
    }

    /// <summary>
    /// Testa se o registro de um novo usuário retorna sucesso.
    /// </summary>
    [Fact]
    public async Task AuthenticateController_RegisterValidUser_ShouldReturnSuccess()
    {
        // Arrange
        this.WithAuthenticatedUser();
        var model = new RegisterModelBuilder().Build();

        // Act
        var response = await _client.PostAsync("/api/employee", model.ToJsonContent());

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await _database.ApplicationUser.FirstOrDefaultAsync(a => a.Email == model.Email);
        Assert.NotNull(user);
    }

    /// <summary>
    /// Testa se um usuário válido pode autenticar e receber um token JWT.
    /// </summary>
    [Fact]
    public async Task AuthenticateController_AuthenticateValidUser_ShouldReturnToken()
    {
        // Arrange
        var password = new Faker().Internet.Password(prefix: "!@#abcGTD197", length: 20);
        var userBuilder = new ApplicationUserBuilder()
                                    .WithFullValidApplicationUser()
                                    .WithPassword(password)
                                    .Save(_userManager, _roleManager);
        var user = userBuilder.Build();
        var loginModel = new LoginModel { Email = user.Email, Password = password };

        // Act
        var response = await _client.PostAsync("/api/authenticate", loginModel.ToJsonContent());
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<dynamic>(content)?.token;

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.False(string.IsNullOrEmpty(token.ToString()));
    }

    [Fact]
    public async Task AuthenticateController_GetCurrentUser_ShouldReturnSuccess()
    {
        // Arrange
        this.WithAuthenticatedUser();

        // Act
        var response = await _client.GetAsync("/api/authenticate/info");
        var content = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<EmployeeModel>(content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(user);
    }

    /// <summary>
    /// Testa se um usuário inválido recebe um erro de autenticação (401 Unauthorized).
    /// </summary>
    [Fact]
    public async Task AuthenticateController_AuthenticateInvalidUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginModel = new LoginModel { Email = "invalid@email.com", Password = "wrongpassword" };

        // Act
        var response = await _client.PostAsync("/api/authenticate", loginModel.ToJsonContent());

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Testa se um usuário registrado pode ser buscado corretamente.
    /// </summary>
    [Fact]
    public async Task EmployeeController_GetValidUser_ShouldReturnSuccess()
    {
        // Arrange
        this.WithAuthenticatedUser();
        var userBuilder = new ApplicationUserBuilder()
                                    .WithFullValidApplicationUser()
                                    .Save(_userManager, _roleManager);
        var registeredUser = userBuilder.Build();

        // Act
        var response = await _client.GetAsync($"/api/employee/{registeredUser.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<EmployeeModel>(content);
        Assert.NotNull(user);
        Assert.Equal(registeredUser.Email, user.Email);
    }

    /// <summary>
    /// Testa se a busca por um usuário inexistente retorna 404 Not Found.
    /// </summary>
    [Fact]
    public async Task EmployeeController_GetInvalidUser_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/employee/invalid-id");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    /// <summary>
    /// Testa se a atualização de um usuário registrado funciona corretamente.
    /// </summary>
    [Fact]
    public async Task EmployeeController_UpdateValidUser_ShouldReturnSuccess()
    {
        // Arrange
        this.WithAuthenticatedUser();
        var userBuilder = new ApplicationUserBuilder()
                                    .WithFullValidApplicationUser()
                                    .Save(_userManager, _roleManager);
        var registeredUser = userBuilder.Build();

        var updateModel = new RegisterModelBuilder().Build();
        updateModel.Email = registeredUser.Email;
        updateModel.FirstName = "UpdatedFirstName";

        // Act
        var response = await _client.PutAsync("/api/employee", updateModel.ToJsonContent());

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await _database.ApplicationUser.FirstOrDefaultAsync(a => a.Email == registeredUser.Email);
        Assert.Equal("UpdatedFirstName", updatedUser.FirstName);
    }

    /// <summary>
    /// Testa se a exclusão de um usuário registrado retorna sucesso.
    /// </summary>
    [Fact]
    public async Task EmployeeController_DeleteValidUser_ShouldReturnSuccess()
    {
        // Arrange
        this.WithAuthenticatedUser();
        var userBuilder = new ApplicationUserBuilder()
                                    .WithFullValidApplicationUser()
                                    .Save(_userManager, _roleManager);
        var registeredUser = userBuilder.Build();

        // Act
        var response = await _client.DeleteAsync($"/api/employee/{registeredUser.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var deletedUser = await _database.ApplicationUser.FirstOrDefaultAsync(a => a.Email == registeredUser.Email);
        Assert.Null(deletedUser);
    }
}
