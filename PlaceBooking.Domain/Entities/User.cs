using PlaceBooking.Domain.Common;

namespace PlaceBooking.Domain.Entities;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    
    // Hash hesla (NIKDY neukládat heslo èistì!)
    public string PasswordHash { get; set; } = string.Empty;
}
