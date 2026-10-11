using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Packages;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public TravelPackage Package { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int? SelectedDepartureId { get; set; }

    [BindProperty]
    public int Travellers { get; set; } = 1;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var pkg = await _context.Packages
            .Include(p => p.Destination)
            .Include(p => p.Itinerary.OrderBy(i => i.DayNumber))
            .Include(p => p.Departures.OrderBy(d => d.StartDate))
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pkg == null) return NotFound();

        Package = pkg;

        if (!SelectedDepartureId.HasValue && Package.Departures.Any(d => d.StartDate >= DateOnly.FromDateTime(DateTime.Today)))
        {
            SelectedDepartureId = Package.Departures
                .Where(d => d.StartDate >= DateOnly.FromDateTime(DateTime.Today))
                .OrderBy(d => d.StartDate)
                .First().Id;
        }

        return Page();
    }
}
