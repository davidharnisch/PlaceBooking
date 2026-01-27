using PlaceBooking.Domain.Common;

namespace PlaceBooking.Domain.Entities;

public class Seat : BaseEntity
{
    public required string Label { get; set; }
    
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    
    // Pro získání historie rezervací daného místa
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
