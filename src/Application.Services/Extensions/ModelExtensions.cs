using Application.Models.Employees;
using Application.Services.Employees;
using Domain.Employees;

namespace Application.Services.Extensions;

public static class ModelExtensions
{
    public static void Update(this ApplicationUser user, RegisterModel model)
    {
        user.Id = Guid.TryParse(model.Id, out var _) ? model.Id : Guid.NewGuid().ToString();
        user.Email = model.Email;
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.DocNumber = model.DocNumber;
        user.DocType = (DocType)model.DocType;
        user.BirthDate = model.BirthDate;
        user.ManagerId = model.ManagerId;
        user.Phones = model.Phones.MapToEmployeePhones();
        user.BirthDate = DateTime.SpecifyKind(model.BirthDate, DateTimeKind.Utc);
    }

    public static bool IsAdult(this RegisterModel model)
    {
        return model.BirthDate <= DateTime.UtcNow.AddYears(-18);
    }

    public static bool IsPhoneNumberAmountValid(this RegisterModel model)
    {
        return model.Phones.Count >= 2;
    }

    public static ApplicationUser ToApplicationUser(this RegisterModel model)
    {
        return new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DocNumber = model.DocNumber,
            DocType = (DocType)model.DocType,
            BirthDate = model.BirthDate,
            ManagerId = model.ManagerId,
        };
    }
}
