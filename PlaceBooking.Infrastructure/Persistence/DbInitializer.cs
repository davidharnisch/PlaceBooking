using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // Look for any users.
        if (context.Users.Any())
        {
            return;   // DB has been seeded
        }

        var users = new User[]
        {
            new User { FirstName = "Jan", LastName = "Novák", Email = "jan.novak@test.com" },
            new User { FirstName = "Petr", LastName = "Svoboda", Email = "petr.svoboda@tulli.com" },
            new User { FirstName = "Admin", LastName = "User", Email = "admin@placebooking.com" }
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
