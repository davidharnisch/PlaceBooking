using PlaceBooking.Domain.Common;

namespace PlaceBooking.Domain.Entities;

public class Booking : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int SeatId { get; set; }
    public Seat? Seat { get; set; }
    
    public DateOnly Date { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
