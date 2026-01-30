using Microsoft.AspNetCore.Mvc;
using PlaceBooking.Application.DTOs;
using PlaceBooking.Application.Services;

namespace PlaceBooking.Web.Controllers;

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
            ViewBag.CurrentUserId = 1; // Hardcoded for MVP (Jan Novák)

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
            // Hardcode UserID for now (simulation of logged in user)
            model.UserId = 1; 

            await _bookingService.CreateBookingAsync(model);
            
            TempData["SuccessMessage"] = "Booking successfully created!";
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
             await _bookingService.CancelBookingAsync(bookingId, userId: 1);
             TempData["SuccessMessage"] = "Booking cancelled.";
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
        // Hardcode UserID for MVP
        int userId = 1; 
        
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

            // Reuse "Room" view but with IsReadOnly flag
            return View("Room", roomDto); 
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }
    }
}
