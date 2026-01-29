namespace PlaceBooking.Application.DTOs;

public class SeatDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
    // If booked, show who booked it (for tooltip)
    public string? BookedBy { get; set; }
    public DateTime? BookedAt { get; set; }
}
