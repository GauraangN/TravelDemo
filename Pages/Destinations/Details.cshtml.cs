using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Destinations;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Destination Destination { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();

        var dest = await _context.Destinations
            .Include(d => d.Packages)
            .ThenInclude(p => p.Departures)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dest == null) return NotFound();

        Destination = dest;
        return Page();
    }
}
