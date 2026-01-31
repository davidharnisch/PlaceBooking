using Microsoft.EntityFrameworkCore;
using PlaceBooking.Application.Common.Interfaces;
using PlaceBooking.Domain.Entities;
using PlaceBooking.Infrastructure.Persistence;

namespace PlaceBooking.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetByDateAndRoomAsync(DateOnly date, int roomId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Seat) // Include info about the seat
            .Where(b => b.Date == date && b.Seat!.RoomId == roomId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Seat)
            .ThenInclude(s => s!.Room)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsForSeatAndDateAsync(int seatId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(b => b.SeatId == seatId && b.Date == date, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.Seat)
            .ThenInclude(s => s!.Room)
            .Where(b => b.Date >= from && b.Date <= to)
            .ToListAsync(cancellationToken);
    }
}
