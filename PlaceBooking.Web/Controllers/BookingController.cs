using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlaceBooking.Application.DTOs;
using PlaceBooking.Application.Services;

namespace PlaceBooking.Web.Controllers;

[Authorize] // Protect entire controller
public class BookingController : Controller
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // 1. Dashboard - Main entry point
    public IActionResult Index()
    {
        return View();
    }

    // 2. Room and Date Selection (Map)
    [HttpGet]
    public async Task<IActionResult> Room(int roomId = 1, string? date = null)
    {
        // Default to today if date not provided
        var selectedDate = string.IsNullOrEmpty(date) 
            ? DateOnly.FromDateTime(DateTime.Today) 
            : DateOnly.Parse(date);

        try
        {
            var roomDto = await _bookingService.GetRoomStateAsync(roomId, selectedDate);
            
            // Pass the date to view via ViewBag to keep it in input
            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.CurrentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            ViewBag.IsReadOnly = false;

            return View(roomDto);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index"); // Or show error
        }
    }

    // 3. Booking Action (called via Form)
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto model)
    {
        try
        {
            // Use logged in user ID
            model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _bookingService.CreateBookingAsync(model);
            
            TempData["SuccessMessage"] = "Rezervace úspìšnì vytvoøena!";
            return RedirectToAction("Room", new { roomId = 1, date = model.Date.ToString("yyyy-MM-dd") });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Room", new { roomId = 1, date = model.Date.ToString("yyyy-MM-dd") });
        }
    }
    
    // 4. Cancel Booking
    [HttpPost]
    public async Task<IActionResult> Cancel(int bookingId, string date)
    {
        try 
        {
             var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
             await _bookingService.CancelBookingAsync(bookingId, userId);
             TempData["SuccessMessage"] = "Rezervace zrušena!";
        }
        catch(Exception ex)
        {
             TempData["ErrorMessage"] = ex.Message;
        }
        
        // If date is provided, return to Map. If not (cancelled from History), return to History.
        if (!string.IsNullOrEmpty(date))
        {
            return RedirectToAction("Room", new { roomId = 1, date = date });
        }
        
        return RedirectToAction("History");
    }

    // 5. My History
    public async Task<IActionResult> History()
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var bookings = await _bookingService.GetMyBookingsAsync(userId);
        return View(bookings);
    }

    // 6. Occupancy Overview (Read Only)
    [HttpGet]
    public async Task<IActionResult> Occupancy(int roomId = 1, string? date = null)
    {
        var selectedDate = string.IsNullOrEmpty(date) 
            ? DateOnly.FromDateTime(DateTime.Today) 
            : DateOnly.Parse(date);

         try
        {
            var roomDto = await _bookingService.GetRoomStateAsync(roomId, selectedDate);
            
            ViewBag.SelectedDate = selectedDate.ToString("yyyy-MM-dd");
            ViewBag.IsReadOnly = true; 
            ViewBag.Title = "Obsazenost Místnosti - Pøehled";
            ViewBag.CurrentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Reuse "Room" view but with IsReadOnly flag
            return View("Room", roomDto); 
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }
    }

    // 7. Usage Statistics
    [HttpGet]
    public async Task<IActionResult> Stats(string? from = null, string? to = null)
    {
         // Default range: First to last day of current month
         var now = DateTime.Today;
         var fromDate = string.IsNullOrEmpty(from) 
             ? new DateOnly(now.Year, now.Month, 1) 
             : DateOnly.Parse(from);
         
         var toDate = string.IsNullOrEmpty(to)
             ? new DateOnly(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month))
             : DateOnly.Parse(to);

         var stats = await _bookingService.GetSeatStatisticsAsync(fromDate, toDate);

         ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
         ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

         return View(stats);
    }
}
