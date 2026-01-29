namespace PlaceBooking.Application.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string SeatLabel { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
}
