using Microsoft.AspNetCore.Identity;

namespace Domain.Employees;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string DocNumber { get; set; }

    public DocType DocType { get; set; }

    public ICollection<UserPhone> Phones { get; set; } = new List<UserPhone>();

    public string ManagerId { get; set; }

    public DateTime BirthDate { get; set; }

    public EmployeeStatus Status { get; set; }

    public ApplicationUser? Manager { get; set; }

    public ApplicationUser()
    {
        Status = EmployeeStatus.Active;
    }

    public void Delete()
    {
        Status = EmployeeStatus.Inactive;
    }
}
