namespace PlaceBooking.Application.DTOs;

public class SeatStatisticsDto
{
    public string RoomName { get; set; } = string.Empty;
    public string SeatLabel { get; set; } = string.Empty;
    public int BookingsCount { get; set; }
}
