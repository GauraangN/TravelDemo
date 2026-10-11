using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Bookings;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<TravelBooking> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        Bookings = await _context.Bookings
            .Include(b => b.Departure)
            .ThenInclude(d => d!.TravelPackage)
            .ThenInclude(p => p!.Destination)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int bookingId)
    {
        var userId = _userManager.GetUserId(User);

        var booking = await _context.Bookings
            .Include(b => b.Departure)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking != null && booking.Status != BookingStatus.Cancelled)
        {
            booking.Status = BookingStatus.Cancelled;

            if (booking.Departure != null)
            {
                booking.Departure.SeatsBooked = Math.Max(0, booking.Departure.SeatsBooked - booking.Travellers);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Reservation successfully cancelled and seats returned to availability.";
        }

        return RedirectToPage();
    }
}
