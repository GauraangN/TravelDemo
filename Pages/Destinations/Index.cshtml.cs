using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Destinations;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Destination> Destinations { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Region { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Destinations
            .Include(d => d.Packages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Region))
        {
            query = query.Where(d => d.Region.ToLower() == Region.Trim().ToLower());
        }

        Destinations = await query.ToListAsync();
    }
}
