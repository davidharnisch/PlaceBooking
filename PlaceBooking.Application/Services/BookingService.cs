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

    public async Task<BookingDto> CreateBookingAsync(int userId, CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        // 1. Validate inputs
        var seat = await _seatRepository.GetByIdAsync(dto.SeatId, cancellationToken);
        if (seat == null)
            throw new Exception($"Místo s ID {dto.SeatId} nebylo nalezeno.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new Exception($"Uživatel s ID {userId} nebyl nalezen.");

        // 2. Check business rules (Concurrency check ideally belongs here or DB constraint)
        // Rule: Is seat free for this date?
        var isBooked = await _bookingRepository.ExistsForSeatAndDateAsync(dto.SeatId, dto.Date, cancellationToken);
        if (isBooked)
            throw new Exception("Toto místo je pro daný den již rezervované.");

        // 3. Create Entity
        var booking = new Booking
        {
            SeatId = dto.SeatId,
            UserId = userId,
            Date = dto.Date,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Save
        try 
        {
            await _bookingRepository.AddAsync(booking, cancellationToken);
        }
        catch (Exception ex)
        {
            // Catch database constraint violation (Unique Index)
            // Note: In real app, check specifically for DbUpdateException and inner exception code (SQLite: 19)
            // Since we added unique index on SeatId + Date, this will fail if simulation race condition occurs.
            if (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE constraint failed"))
            {
                throw new Exception("Toto místo je pro daný den již rezervované (souèasná rezervace).");
            }
            throw; // Rethrow other errors
        }
        
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
            throw new Exception("Rezervace nebyla nalezena.");

        // Rule: Can cancel only own booking
        if (booking.UserId != userId)
            throw new Exception("Mùžete zrušit pouze svou vlastní rezervaci.");

        // Rule: Cannot cancel past bookings (optional, based on req)
        // "To mùže maximálnì v ten daný den, nikoliv zpìtnì." -> Can cancel today or future.
        if (booking.Date < DateOnly.FromDateTime(DateTime.UtcNow))
             throw new Exception("Nelze zrušit rezervace v minulosti.");

        await _bookingRepository.DeleteAsync(booking, cancellationToken);
    }

    public async Task<RoomDto> GetRoomStateAsync(int roomId, DateOnly date, CancellationToken cancellationToken = default)
    {
        // 1. Get Room with Seats
        var room = await _roomRepository.GetRoomWithSeatsAsync(roomId, cancellationToken);
        if (room == null)
             throw new Exception($"Místnost s ID {roomId} nebyla nalezena.");

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
                BookedAt = booking?.CreatedAt,
                BookingId = booking?.Id,
                BookedByUserId = booking?.UserId
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
            SeatLabel = b.Seat?.Label ?? "Neznámé",
            RoomName = b.Seat?.Room?.Name ?? "Neznámé",
            UserName = $"{b.User?.FirstName} {b.User?.LastName}", // Should be current user
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<IEnumerable<SeatStatisticsDto>> GetSeatStatisticsAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        // 1. Get raw bookings
        var bookings = await _bookingRepository.GetByDateRangeAsync(from, to, cancellationToken);

        // 2. Group by Seat
        // We want to count how many bookings each seat has in this period.
        var grouped = bookings
            .GroupBy(b => b.SeatId)
            .Select(g => new
            {
                SeatId = g.Key,
                Count = g.Count(),
                // Take the first booking to get Seat info (Relationship is preserved in all items)
                FirstBooking = g.FirstOrDefault() 
            })
            .OrderByDescending(x => x.Count);

        // 3. Map to DTO
        var result = new List<SeatStatisticsDto>();
        foreach (var group in grouped)
        {
            if (group.FirstBooking?.Seat == null) continue;

            result.Add(new SeatStatisticsDto
            {
                RoomName = group.FirstBooking.Seat.Room?.Name ?? "Neznámé",
                SeatLabel = group.FirstBooking.Seat.Label ?? "Neznámé",
                BookingsCount = group.Count
            });
        }

        return result;
    }
}
