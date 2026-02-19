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
            new User { FirstName = "Jan", LastName = "Novák", Email = "jan.novak@test.com", PasswordHash = defaultHash }
        };

        foreach (var u in users)
        {
            context.Users.Add(u);
        }
        context.SaveChanges();

        var rooms = new Room[]
        {
            new Room { Name = "TULLI" },
            new Room { Name = "eCustoms" },
            new Room { Name = "TMA" }
        };

        foreach (var r in rooms)
        {
            context.Rooms.Add(r);
        }
        context.SaveChanges();
        
        // Add seats to Tulli
        var tulli = rooms[0];
        var seats = new List<Seat>();
        
        for (int i = 1; i <= 24; i++)
        {
            seats.Add(new Seat 
            { 
                Label = $"TULLI-{i}", 
                RoomId = tulli.Id 
            });
        }
        
        // Add seats to eCustoms
        var eCustoms = rooms[1];
        for (int i = 1; i <= 18; i++)
        {
             seats.Add(new Seat 
            { 
                Label = $"eCustoms-{i}", 
                RoomId = eCustoms.Id 
            });
        }

        // Add seats to TMA
        var tma = rooms[2];
        for (int i = 1; i <= 30; i++)
        {
            seats.Add(new Seat
            {
                Label = $"TMA-{i}",
                RoomId = tma.Id
            });
        }

        foreach (var s in seats)
        {
            context.Seats.Add(s);
        }
        context.SaveChanges();
    }
}
