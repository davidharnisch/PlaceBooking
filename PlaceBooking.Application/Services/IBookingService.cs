using PlaceBooking.Application.DTOs;

namespace PlaceBooking.Application.Services;

public interface IBookingService
{
    // Returns true if success, throws exception if validation fails
    Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, CancellationToken cancellationToken = default);
    
    // Cancel booking (logic: can cancel only own booking and only for today/future)
    Task CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default);

    // Get room state (layout + booking info) for a specific date
    // This is for the "Book a place" view
    Task<RoomDto> GetRoomStateAsync(int roomId, DateOnly date, CancellationToken cancellationToken = default);
}
