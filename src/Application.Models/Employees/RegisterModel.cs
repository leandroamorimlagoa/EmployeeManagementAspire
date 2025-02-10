
using System.ComponentModel.DataAnnotations;

namespace Application.Models.Employees;

public class RegisterModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string DocNumber { get; set; }
    
    [Required]
    public short DocType { get; set; }
    
    [Required]
    public List<RegisterPhoneModel> Phones { get; set; } = new List<RegisterPhoneModel>();
    
    [Required]
    public DateTime BirthDate { get; set; }
    
    [Required]
    public string ManagerId { get; set; }
        
    [Required]
    public string Password { get; set; }
    
    [Required]
    public short Role { get; set; }
}
