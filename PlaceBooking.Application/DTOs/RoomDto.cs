namespace PlaceBooking.Application.DTOs;

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SeatDto> Seats { get; set; } = new();
}
