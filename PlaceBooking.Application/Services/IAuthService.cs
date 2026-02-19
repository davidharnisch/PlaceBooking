using PlaceBooking.Application.DTOs;

namespace PlaceBooking.Application.Services;

public interface IAuthService
{
    // Register new user, hashes password, saves to DB
    Task RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);

    // Verify credentials, returns UserDto (without password) or null
    Task<UserDto?> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}
