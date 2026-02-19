namespace PlaceBooking.Application.DTOs;

public class CreateBookingDto
{
    public int UserId { get; set; } // Temporary until we have real auth
    public int SeatId { get; set; }
    public int RoomId { get; set; }
    public DateOnly Date { get; set; }
}
