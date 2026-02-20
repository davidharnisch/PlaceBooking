namespace PlaceBooking.Application.DTOs;

public class CreateBookingDto
{
    public int SeatId { get; set; }
    public int RoomId { get; set; }
    public DateOnly Date { get; set; }
}
