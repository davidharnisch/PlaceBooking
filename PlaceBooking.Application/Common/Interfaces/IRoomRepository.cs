using PlaceBooking.Domain.Entities;

namespace PlaceBooking.Application.Common.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    // Specific method to load a room with all its seats
    Task<Room?> GetRoomWithSeatsAsync(int id, CancellationToken cancellationToken = default);
}
