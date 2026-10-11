using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Bookings;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CreateModel(AppDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public TravelBooking Booking { get; set; } = new();

    public Departure Departure { get; set; } = default!;
    public TravelPackage Package { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int departureId, int travellers = 1)
    {
        var dep = await _context.Departures
            .Include(d => d.TravelPackage)
            .ThenInclude(p => p!.Destination)
            .FirstOrDefaultAsync(d => d.Id == departureId);

        if (dep == null || dep.TravelPackage == null)
        {
            return RedirectToPage("/Packages/Index");
        }

        Departure = dep;
        Package = dep.TravelPackage;

        var currentUser = await _userManager.GetUserAsync(User);

        Booking = new TravelBooking
        {
            DepartureId = departureId,
            Travellers = Math.Max(1, Math.Min(travellers, dep.SeatsLeft)),
            LeadName = currentUser?.UserName ?? "",
            Phone = ""
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dep = await _context.Departures
            .Include(d => d.TravelPackage)
            .ThenInclude(p => p!.Destination)
            .FirstOrDefaultAsync(d => d.Id == Booking.DepartureId);

        if (dep == null || dep.TravelPackage == null)
        {
            ModelState.AddModelError("", "Selected departure is invalid.");
            return Page();
        }

        Departure = dep;
        Package = dep.TravelPackage;

        if (Booking.Travellers <= 0)
        {
            ModelState.AddModelError("Booking.Travellers", "Please select at least 1 traveller.");
            return Page();
        }

        if (dep.SeatsLeft < Booking.Travellers)
        {
            ModelState.AddModelError("", $"Only {dep.SeatsLeft} seat(s) remaining for this departure date.");
            return Page();
        }

        var userId = _userManager.GetUserId(User) ?? "";

        // Reserve seats with optimistic concurrency check
        dep.SeatsBooked += Booking.Travellers;

        Booking.UserId = userId;
        Booking.TotalPrice = Package.PricePerPerson * Booking.Travellers;
        Booking.Status = BookingStatus.Confirmed;
        Booking.CreatedAt = DateTime.UtcNow;

        _context.Bookings.Add(Booking);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "A seat availability conflict occurred because another traveler just booked the remaining seats. Please try selecting a different departure date.");
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Booking error: {ex.Message}");
            return Page();
        }

        return RedirectToPage("/Bookings/Confirmation", new { id = Booking.Id });
    }
}
