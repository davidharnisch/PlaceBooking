using PlaceBooking.Application.DTOs;

namespace PlaceBooking.Application.Services;

public interface IAuthService
{
    // Register new user, hashes password, saves to DB
    Task RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);

    // Verify credentials, returns UserDto (without password) or null
    Task<UserDto?> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}

// Simple DTO for User result to keep Domain Entity hidden from Controller
public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
