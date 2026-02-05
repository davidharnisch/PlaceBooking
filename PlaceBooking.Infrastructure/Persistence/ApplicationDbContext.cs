using Microsoft.EntityFrameworkCore;
using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Entity Relationships and Constraints

        // Room -> Seats (One-to-Many)
        modelBuilder.Entity<Room>()
            .HasMany(r => r.Seats)
            .WithOne(s => s.Room)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seat -> Bookings (One-to-Many)
        modelBuilder.Entity<Seat>()
            .HasMany(s => s.Bookings)
            .WithOne(b => b.Seat)
            .HasForeignKey(b => b.SeatId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting seat if bookings exist

        // Concurrent Booking Protection: Unique Index on SeatId + Date
        // This ensures the database physically rejects duplicate bookings.
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.SeatId, b.Date })
            .IsUnique();

        // User -> Bookings (One-to-Many)
        // Note: User entity doesn't have a Bookings collection defined yet in Domain, 
        // but we can configure the relationship from Booking side.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
