using PlaceBooking.Application.Common.Interfaces;
using PlaceBooking.Application.DTOs;
using PlaceBooking.Application.Services;
using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IRepository<User> _userRepository; // Generic repo for User
    private readonly IRepository<Seat> _seatRepository; // Generic repo for Seat

    public BookingService(
        IBookingRepository bookingRepository,
        IRoomRepository roomRepository,
        IRepository<User> userRepository,
        IRepository<Seat> seatRepository)
    {
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _seatRepository = seatRepository;
    }

    public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validate inputs
        var seat = await _seatRepository.GetByIdAsync(dto.SeatId, cancellationToken);
        if (seat == null)
            throw new Exception($"Seat with ID {dto.SeatId} not found.");

        var user = await _userRepository.GetByIdAsync(dto.UserId, cancellationToken);
        if (user == null)
            throw new Exception($"User with ID {dto.UserId} not found.");

        // 2. Check business rules (Concurrency check ideally belongs here or DB constraint)
        // Rule: Is seat free for this date?
        var isBooked = await _bookingRepository.ExistsForSeatAndDateAsync(dto.SeatId, dto.Date, cancellationToken);
        if (isBooked)
            throw new Exception("Seat is already booked for this date.");

        // 3. Create Entity
        var booking = new Booking
        {
            SeatId = dto.SeatId,
            UserId = dto.UserId,
            Date = dto.Date,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Save
        await _bookingRepository.AddAsync(booking, cancellationToken);
        
        // 5. Return DTO
        return new BookingDto
        {
            Id = booking.Id,
            SeatLabel = seat.Label,
            UserName = $"{user.FirstName} {user.LastName}",
            Date = booking.Date,
            CreatedAt = booking.CreatedAt
        };
    }

    public async Task CancelBookingAsync(int bookingId, int userId, CancellationToken cancellationToken = default)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking == null)
            throw new Exception("Booking not found.");

        // Rule: Can cancel only own booking
        if (booking.UserId != userId)
            throw new Exception("You can only cancel your own bookings.");

        // Rule: Cannot cancel past bookings (optional, based on req)
        // "To mùže maximálnì v ten daný den, nikoliv zpìtnì." -> Can cancel today or future.
        if (booking.Date < DateOnly.FromDateTime(DateTime.UtcNow))
             throw new Exception("Cannot cancel past bookings.");

        await _bookingRepository.DeleteAsync(booking, cancellationToken);
    }

    public async Task<RoomDto> GetRoomStateAsync(int roomId, DateOnly date, CancellationToken cancellationToken = default)
    {
        // 1. Get Room with Seats
        var room = await _roomRepository.GetRoomWithSeatsAsync(roomId, cancellationToken);
        if (room == null)
             throw new Exception($"Room with ID {roomId} not found.");

        // 2. Get Bookings for this room and date
        var bookings = await _bookingRepository.GetByDateAndRoomAsync(date, roomId, cancellationToken);
        
        // 3. Map to DTO
        var seatDtos = new List<SeatDto>();
        foreach (var seat in room.Seats)
        {
            // Find if this seat has a booking in the list
            var booking = bookings.FirstOrDefault(b => b.SeatId == seat.Id);
            
            var seatDto = new SeatDto
            {
                Id = seat.Id,
                Label = seat.Label,
                IsBooked = booking != null,
                BookedBy = booking != null ? $"{booking.User?.FirstName} {booking.User?.LastName}" : null,
                BookedAt = booking?.CreatedAt
            };
            seatDtos.Add(seatDto);
        }

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Seats = seatDtos
        };
    }

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId, cancellationToken);
        
        // Map to DTOs
        return bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            Date = b.Date,
            SeatLabel = b.Seat?.Label ?? "Unknown",
            RoomName = b.Seat?.Room?.Name ?? "Unknown",
            UserName = $"{b.User?.FirstName} {b.User?.LastName}", // Should be current user
            CreatedAt = b.CreatedAt
        });
    }
}
