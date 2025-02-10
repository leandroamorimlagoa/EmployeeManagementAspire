using Application.Models.Employees;
using Bogus;
using Domain.Employees;

namespace IntegrationTests.Builders.Models;

internal class RegisterModelBuilder
{
    private RegisterModel registerModel;
    private Faker faker = new Faker();

    /// <summary>
    /// Default RegisterModel is an Employee, with a Passport as document type, no manager, more than 18 years old
    /// </summary>
    public RegisterModelBuilder()
    {
        WithDefaultValidRegisterModel();
    }

    private void WithDefaultValidRegisterModel()
    => registerModel = new RegisterModel
    {
        Email = faker.Person.Email,
        FirstName = faker.Person.FirstName,
        LastName = faker.Person.LastName,
        DocNumber = faker.Random.AlphaNumeric(10),
        DocType = (short)DocType.Passport,
        BirthDate = DateTime.UtcNow.AddYears(-19),
        ManagerId = null,
        Password = faker.Internet.Password(prefix: "!@#abcGTD197", length: 20),
        Role = (short)Role.Employee,
        Phones = new List<RegisterPhoneModel>
            {
                new RegisterPhoneModel
                {
                    PhoneNumber = faker.Phone.PhoneNumber("####-####")
                },
                new RegisterPhoneModel
                {
                    PhoneNumber = faker.Phone.PhoneNumber("####-####")
                }
            }
    };

    public RegisterModel Build()
    {
        return registerModel;
    }
}
