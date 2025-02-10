namespace Application.Models.Employees;

public class EmployeeModel
{
    public string Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string DocNumber { get; set; } = string.Empty;

    public short DocType { get; set; } = 0;

    public IEnumerable<RegisterPhoneModel> Phones { get; set; } = new List<RegisterPhoneModel>();

    public string Email { get; set; } = string.Empty;

    public string BirthDate { get; set; } = string.Empty;

    public string? ManagerId { get; set; }

    public string? Role { get; set; }
}
