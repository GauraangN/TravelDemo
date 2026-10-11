using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Admin;

[Authorize(Roles = "Admin")]
public class PackagesModel : PageModel
{
    private readonly AppDbContext _context;

    public PackagesModel(AppDbContext context)
    {
        _context = context;
    }

    public List<TravelPackage> Packages { get; set; } = new();
    public List<Destination> Destinations { get; set; } = new();

    [BindProperty]
    public TravelPackage NewPackage { get; set; } = new();

    [BindProperty]
    public string InitialDepartureDate { get; set; } = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd");

    public async Task OnGetAsync()
    {
        Destinations = await _context.Destinations.ToListAsync();
        Packages = await _context.Packages
            .Include(p => p.Destination)
            .Include(p => p.Departures)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (NewPackage.DestinationId <= 0)
        {
            ModelState.AddModelError("NewPackage.DestinationId", "Please select a valid destination.");
            await OnGetAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(NewPackage.Title))
        {
            ModelState.AddModelError("NewPackage.Title", "Title is required.");
            await OnGetAsync();
            return Page();
        }

        if (string.IsNullOrWhiteSpace(NewPackage.ImageUrl))
        {
            NewPackage.ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80";
        }

        _context.Packages.Add(NewPackage);
        await _context.SaveChangesAsync();

        // Create an initial departure date
        if (DateOnly.TryParse(InitialDepartureDate, out var depDate))
        {
            var initialDep = new Departure
            {
                TravelPackageId = NewPackage.Id,
                StartDate = depDate,
                SeatsTotal = NewPackage.MaxGroupSize > 0 ? NewPackage.MaxGroupSize : 10,
                SeatsBooked = 0
            };
            _context.Departures.Add(initialDep);
            await _context.SaveChangesAsync();
        }

        TempData["AdminSuccess"] = $"Package '{NewPackage.Title}' created successfully!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int packageId)
    {
        var pkg = await _context.Packages
            .Include(p => p.Departures)
            .Include(p => p.Itinerary)
            .FirstOrDefaultAsync(p => p.Id == packageId);

        if (pkg != null)
        {
            _context.Packages.Remove(pkg);
            await _context.SaveChangesAsync();
            TempData["AdminSuccess"] = "Package deleted successfully.";
        }

        return RedirectToPage();
    }
}
