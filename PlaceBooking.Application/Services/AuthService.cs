using BCrypt.Net;
using PlaceBooking.Application.Common.Interfaces;
using PlaceBooking.Application.DTOs;
using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;

    public AuthService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Check if email exists
        var existing = await _userRepository.FindAsync(u => u.Email == dto.Email, cancellationToken);
        if (existing.Any())
        {
            throw new Exception("Uživatel s tímto emailem již existuje.");
        }

        // 2. Hash password
        // BCrypt handles salt generation automatically.
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 3. Create Entity
        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = passwordHash
        };

        // 4. Save
        await _userRepository.AddAsync(user, cancellationToken);
    }

    public async Task<UserDto?> VerifyCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email, cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null) return null; // User not found

        // Verify password
        // Handle legacy users (seed data) who might not have a hash yet (optional safety)
        if (string.IsNullOrEmpty(user.PasswordHash)) 
        {
             // For safety, deny login if no password set, or strictly require hash
             return null;
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        
        if (!isValid) return null;

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}
