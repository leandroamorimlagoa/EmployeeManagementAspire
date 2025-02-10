using Application.Models.Employees;
using Application.Services.Employees;
using Domain.Core;
using Domain.Employees;
using Domain.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly NotificationContext _notificationContext;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly AuthenticatedUser _authenticatedUser;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _authenticatedUser = new AuthenticatedUser();
        _notificationContext = new NotificationContext();

        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

        _userService = new UserService(
            _userRepositoryMock.Object,
            _roleManagerMock.Object,
            _notificationContext,
            _authenticatedUser,
            _loggerMock.Object);
    }

    /// <summary>
    /// Testa se um usuário válido pode ser registrado com sucesso.
    /// </summary>
    [Fact]
    public async Task RegisterUserAsync_ValidUser_ShouldCreateUser()
    {
        // Arrange
        var model = new RegisterModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DocNumber = "12345678901",
            DocType = (short)DocType.CPF,
            BirthDate = DateTime.UtcNow.AddYears(-20),
            Password = "SecureP@ssword!",
            Role = (short)Role.Employee,
            Phones = new List<RegisterPhoneModel> 
            { 
                new RegisterPhoneModel { PhoneNumber = "999999999" },
                new RegisterPhoneModel { PhoneNumber = "888888888" }
            }
        };

        _userRepositoryMock
            .Setup(repo => repo.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock
            .Setup(roleManager => roleManager.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _authenticatedUser.Role = Role.Director.ToString();

        // Act
        await _userService.RegisterUserAsync(model);

        // Assert
        Assert.False(_notificationContext.HasNotifications);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    /// Testa se um usuário menor de idade não pode ser registrado.
    /// </summary>
    [Fact]
    public async Task RegisterUserAsync_UserUnder18_ShouldNotCreateUser()
    {
        // Arrange
        var model = new RegisterModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            BirthDate = DateTime.UtcNow.AddYears(-16),
            Password = "SecureP@ssword!",
            Role = (short)Role.Employee
        };

        // Act
        await _userService.RegisterUserAsync(model);

        // Assert
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains("User must be at least 18 years old.", _notificationContext.Notifications.Select(n => n.Message));
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Testa se um usuário inválido não é criado.
    /// </summary>
    [Fact]
    public async Task RegisterUserAsync_InvalidUser_ShouldNotCreateUser()
    {
        // Arrange
        var model = new RegisterModel();

        // Act
        await _userService.RegisterUserAsync(model);

        // Assert
        Assert.True(_notificationContext.HasNotifications);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Testa se um usuário pode ser atualizado corretamente.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_UserExists_ShouldUpdateUser()
    {
        // Arrange
        var model = new RegisterModel
        {
            Email = "test@example.com",
            FirstName = "Updated",
            LastName = "User",
            DocNumber = "12345678901",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            Password = "SecureP@ssword!",
            Role = (short)Role.Employee,
            Phones = new List<RegisterPhoneModel>
            {
                new RegisterPhoneModel { PhoneNumber = "999999999" },
                new RegisterPhoneModel { PhoneNumber = "888888888" }
            }
        };

        var existingUser = new ApplicationUser
        {
            Email = model.Email,
            FirstName = "John",
            LastName = "Doe",
            DocNumber = "12345678901",
            BirthDate = DateTime.UtcNow.AddYears(-25)
        };

        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(model.Email))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repo => repo.UpdateUserAsync(existingUser))
            .ReturnsAsync(IdentityResult.Success);

        _authenticatedUser.Role = Role.Director.ToString();

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        Assert.False(_notificationContext.HasNotifications);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(existingUser), Times.Once);
    }

    /// <summary>
    /// Testa se um usuário inexistente não pode ser atualizado.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ShouldNotUpdateUser()
    {
        // Arrange
        var model = new RegisterModel
        {
            Email = "notfound@example.com",
            FirstName = "Updated",
            LastName = "User",
            DocNumber = "12345678901",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            Password = "SecureP@ssword!",
            Role = (short)Role.Employee,
            Phones = new List<RegisterPhoneModel>
            {
                new RegisterPhoneModel { PhoneNumber = "999999999" },
                new RegisterPhoneModel { PhoneNumber = "888888888" }
            }
        };

        _userRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(model.Email))
            .ReturnsAsync((ApplicationUser)null);

        _authenticatedUser.Role = Role.Director.ToString();

        // Act
        await _userService.UpdateUserAsync(model);

        // Assert
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains("User not found", _notificationContext.Notifications.Select(n => n.Message));
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    /// <summary>
    /// Testa se um usuário pode ser deletado corretamente.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_UserExists_ShouldMarkAsInactive()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "123",
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DocNumber = "12345678901",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            Status = EmployeeStatus.Active
        };

        _userRepositoryMock
            .Setup(repo => repo.GetUserByIdAsync(user.Id))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(repo => repo.UpdateUserAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.DeleteUserAsync(user.Id);

        // Assert
        Assert.False(_notificationContext.HasNotifications);
        Assert.Equal(EmployeeStatus.Inactive, user.Status);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
    }
}
