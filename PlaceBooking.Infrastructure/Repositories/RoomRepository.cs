using Microsoft.EntityFrameworkCore;
using PlaceBooking.Application.Common.Interfaces;
using PlaceBooking.Domain.Entities;
using PlaceBooking.Infrastructure.Persistence;

namespace PlaceBooking.Infrastructure.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Room?> GetRoomWithSeatsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.Seats)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
