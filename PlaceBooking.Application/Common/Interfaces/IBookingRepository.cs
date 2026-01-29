using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Application.Common.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    // "Find all bookings for a given date and room"
    Task<IEnumerable<Booking>> GetByDateAndRoomAsync(DateOnly date, int roomId, CancellationToken cancellationToken = default);
    
    // "Find user bookings"
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    // "Does a booking exist for this seat and date?"
    Task<bool> ExistsForSeatAndDateAsync(int seatId, DateOnly date, CancellationToken cancellationToken = default);
}
