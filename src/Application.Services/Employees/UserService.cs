using Application.Models.Employees;
using Application.Services.Extensions;
using Domain.Core;
using Domain.Employees;
using Domain.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services.Employees;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly NotificationContext notificationContext;
    private readonly AuthenticatedUser authenticatedUser;
    private readonly ILogger<UserService> logger;

    public UserService(IUserRepository userRepository,
                        RoleManager<IdentityRole> roleManager,
                        NotificationContext notificationContext,
                        AuthenticatedUser authenticatedUser,
                        ILogger<UserService> logger)
    {
        this.userRepository = userRepository;
        this.roleManager = roleManager;
        this.notificationContext = notificationContext;
        this.authenticatedUser = authenticatedUser;
        this.logger = logger;
    }

    public async Task RegisterUserAsync(RegisterModel model)
    {
        if (!IsValid(model))
        {
            return;
        }

        var newUser = model.ToApplicationUser();
        newUser.Update(model);

        try
        {

            var roleName = ((Role)model.Role).ToString();
            if (!await this.roleManager.RoleExistsAsync(roleName))
            {
                await this.roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var identity = await userRepository.CreateUserAsync(newUser, model.Password, model.Role.ToString());
            if (!identity.Succeeded)
            {
                foreach (var error in identity.Errors)
                {
                    notificationContext.AddNotification(error.Description);
                }
            }
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while creating user");
            notificationContext.AddNotification($"Error while creating user, check the logs:{err.Message}");
        }
    }

    public async Task UpdateUserAsync(RegisterModel model)
    {
        if (!IsValid(model))
        {
            return;
        }
        var user = await userRepository.GetUserByEmailAsync(model.Email);
        if (user == null)
        {
            notificationContext.AddNotification("User not found");
            return;
        }
        user.Update(model);
        await UpdateUser(user);
    }

    public async Task DeleteUserAsync(string id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            notificationContext.AddNotification("User not found");
            return;
        }
        user.Delete();
        await UpdateUser(user);
    }

    private async Task UpdateUser(ApplicationUser user)
    {
        try
        {
            var result = await userRepository.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    notificationContext.AddNotification(error.Description);
                }
            }
        }
        catch (Exception err)
        {
            logger.LogError(err, "Error while updating user");
            notificationContext.AddNotification($"Error while updating user, check the logs:{err.Message}");
        }
    }

    private bool IsValid(RegisterModel model)
    {
        if (!model.IsAdult())
        {
            notificationContext.AddNotification("User must be at least 18 years old.");
        }

        if (!IsValidRoleForEmployee(model))
        {
            notificationContext.AddNotification("The current user cannot create employee with this role.");
        }

        if (!model.IsPhoneNumberAmountValid())
        {
            notificationContext.AddNotification("Invalid phone number");
        }

        if (model.Phones.Count() > 0)
        {
            foreach (var phone in model.Phones)
            {
                if (string.IsNullOrEmpty(phone.PhoneNumber))
                {
                    notificationContext.AddNotification($"Invalid phone number: {phone.PhoneNumber}");
                }
            }
        }

        return !notificationContext.HasNotifications;
    }

    private bool IsValidRoleForEmployee(RegisterModel model)
    {
        if (Enum.TryParse(authenticatedUser.Role, out Role authenticatedRole))
        {
            switch (authenticatedRole)
            {
                case Role.Employee:
                    return model.Role == (short)Role.Employee;
                case Role.Leader:
                    return model.Role == (short)Role.Employee || model.Role == (short)Role.Leader;
                case Role.Director:
                    return true;
                default:
                    break;
            }
        }
        logger.LogError("Invalid role: {Role}", authenticatedUser.Role);
        notificationContext.AddNotification($"Invalid role: {authenticatedUser.Role}");
        return false;
    }

    public async Task<EmployeeModel?> GetUserByIdAsync(string id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            notificationContext.AddNotification("User not found");
            return null;
        }
        try
        {
            var phones = await userRepository.GetUserPhonesAsync(user.Id);
            var role = await userRepository.GetUserRoleAsync(user);
            var model = user.MapToEmployeeModel();
            model.Phones = phones.MapToEmployeePhonesModel();
            model.Role = role;
            return model;
        }
        catch (Exception err)
        {

        }
        return null;
    }

    public async Task<IEnumerable<EmployeeModel>> GetUsersAsync(EmployeeModelFilter filterModel)
    => (await userRepository.GetUsersAsync(filterModel.MapToEmployeeFilter())).MapToEmployeeModelList();
}
