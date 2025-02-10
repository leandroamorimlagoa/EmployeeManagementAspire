namespace Domain.Employees;

public class UserPhone
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
