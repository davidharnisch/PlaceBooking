using PlaceBooking.Domain.Common;

namespace PlaceBooking.Domain.Entities;

public class Seat : BaseEntity
{
    public required string Label { get; set; }
    
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    
    // To get booking history for this seat
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
