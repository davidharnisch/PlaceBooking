using PlaceBooking.Domain.Common;

namespace PlaceBooking.Domain.Entities;

public class Room : BaseEntity
{
    public required string Name { get; set; }
    
    // Navigation property
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
