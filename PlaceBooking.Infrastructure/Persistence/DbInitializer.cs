using Microsoft.EntityFrameworkCore; // Needed for Migrate()
using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // APPLY MIGRATIONS automatically
        // This replaces EnsureCreated() which skips migrations history
        context.Database.Migrate();

        // Look for any users.x
        if (context.Users.Any())
        {
            return;   // DB has been seeded
        }

        // Default password for seed users: "heslo123"
        // In real app, we would use AuthService, but here we can't inject it easily (static method),
        // so we just manually hash it using BCrypt directly.
        string defaultHash = BCrypt.Net.BCrypt.HashPassword("heslo123");

        var users = new User[]
        {
            new User { FirstName = "Jan", LastName = "Novák", Email = "jan.novak@test.com", PasswordHash = defaultHash },
            new User { FirstName = "Petr", LastName = "Svoboda", Email = "petr.svoboda@tulli.com", PasswordHash = defaultHash },
            new User { FirstName = "Admin", LastName = "User", Email = "admin@placebooking.com", PasswordHash = defaultHash }
        };

        foreach (var u in users)
        {
            context.Users.Add(u);
        }
        context.SaveChanges();

        var rooms = new Room[]
        {
            new Room { Name = "Sever" },
            new Room { Name = "Apollo" }
        };

        foreach (var r in rooms)
        {
            context.Rooms.Add(r);
        }
        context.SaveChanges();
        
        // Add seats to Sever
        var sever = rooms[0];
        var seats = new List<Seat>();
        
        for (int i = 1; i <= 20; i++)
        {
            seats.Add(new Seat 
            { 
                Label = $"Sever-{i}", 
                RoomId = sever.Id 
            });
        }
        
        // Add seats to Blue Zone
        var apollo = rooms[1];
        for (int i = 1; i <= 15; i++)
        {
             seats.Add(new Seat 
            { 
                Label = $"Apollo-{i}", 
                RoomId = apollo.Id 
            });
        }

        foreach (var s in seats)
        {
            context.Seats.Add(s);
        }
        context.SaveChanges();
    }
}
