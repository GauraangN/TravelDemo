using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalDestinations { get; set; }
    public int TotalPackages { get; set; }

    public List<TravelBooking> AllBookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Confirmed);
        TotalRevenue = (decimal)await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .SumAsync(b => b.TotalPrice);

        TotalDestinations = await _context.Destinations.CountAsync();
        TotalPackages = await _context.Packages.CountAsync();

        AllBookings = await _context.Bookings
            .Include(b => b.Departure)
            .ThenInclude(d => d!.TravelPackage)
            .ThenInclude(p => p!.Destination)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int bookingId, BookingStatus newStatus)
    {
        var booking = await _context.Bookings
            .Include(b => b.Departure)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking != null)
        {
            var oldStatus = booking.Status;
            booking.Status = newStatus;

            // Adjust departure seat counters if status changes to/from Cancelled
            if (oldStatus != BookingStatus.Cancelled && newStatus == BookingStatus.Cancelled && booking.Departure != null)
            {
                booking.Departure.SeatsBooked = Math.Max(0, booking.Departure.SeatsBooked - booking.Travellers);
            }
            else if (oldStatus == BookingStatus.Cancelled && newStatus != BookingStatus.Cancelled && booking.Departure != null)
            {
                booking.Departure.SeatsBooked += booking.Travellers;
            }

            await _context.SaveChangesAsync();
            TempData["AdminSuccess"] = $"Booking #WND-{booking.Id:D6} status updated to {newStatus}.";
        }

        return RedirectToPage();
    }
}
