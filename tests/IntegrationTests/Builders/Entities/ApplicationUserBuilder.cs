using Application.Models.Employees;
using Bogus;
using Domain.Employees;
using Microsoft.AspNetCore.Identity;

namespace IntegrationTests.Builders.Entities;

public class ApplicationUserBuilder
{
    private ApplicationUser _applicationUser;
    private string role = Role.Employee.ToString();
    private Faker _faker;
    private string password;

    /// <summary>
    /// Default is ValidUser
    /// </summary>
    public ApplicationUserBuilder()
    {
        this._faker = new Faker();
        this._applicationUser = new ApplicationUser();
        WithFullValidApplicationUser();
    }

    public ApplicationUserBuilder WithFullValidApplicationUser()
    {
        var email = _faker.Internet.Email();

        _applicationUser.Id = _faker.Random.Guid().ToString();
        _applicationUser.Email = email;
        _applicationUser.UserName = email;
        _applicationUser.FirstName = _faker.Person.FirstName;
        _applicationUser.LastName = _faker.Person.LastName;
        _applicationUser.DocNumber = _faker.Random.String2(2, "ABCDEFGHIJLMNOPKRSTUVXZ") + _faker.Random.String2(6, "0123456789");
        _applicationUser.DocType = DocType.Passport;
        _applicationUser.BirthDate = DateTime.Now.AddYears(-19);
        _applicationUser.Status = EmployeeStatus.Active;
        WithValidPhoneNumbers();

        return this;
    }

    public ApplicationUserBuilder Save(UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager)
    {
        if (string.IsNullOrEmpty(password))
        {
            password = new Faker().Internet.Password(prefix: "!@#abcGTD197", length: 20);
        }

        var result = manager.CreateAsync(_applicationUser, password).Result;
        if (!result.Succeeded)
        {
            throw new Exception("Error on save user");
        }

        if (string.IsNullOrEmpty(role))
        {
            return this;
        }
        if (!roleManager.RoleExistsAsync(role).Result)
        {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
        }
        var roleResult = manager.AddToRoleAsync(_applicationUser, role).Result;
        if (!roleResult.Succeeded)
        {
            throw new Exception("Error on save user role");
        }
        return this;
    }

    internal ApplicationUserBuilder WithPassword(string password)
    {
        this.password = password;
        return this;
    }

    public string GetPassword()
    {
        return password;
    }

    public ApplicationUser Build()
    {
        return _applicationUser;
    }

    private void WithValidPhoneNumbers(int phoneAmout = 2)
    {
        var phones = new List<UserPhone>();
        for (int i = 0; i < phoneAmout; i++)
        {
            var phone = new UserPhone
            {
                PhoneNumber = _faker.Phone.PhoneNumber()
            };
            if (!string.IsNullOrEmpty(_applicationUser.Id))
            {
                phone.UserId = _applicationUser.Id;
            }
            phones.Add(phone);
        }
        _applicationUser.Phones = phones;
    }
}
