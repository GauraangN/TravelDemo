using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Bookings;

[Authorize]
public class ConfirmationModel : PageModel
{
    private readonly AppDbContext _context;

    public ConfirmationModel(AppDbContext context)
    {
        _context = context;
    }

    public TravelBooking Booking { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Departure)
            .ThenInclude(d => d!.TravelPackage)
            .ThenInclude(p => p!.Destination)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        Booking = booking;
        return Page();
    }
}
