using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages.Packages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<TravelPackage> Packages { get; set; } = new();
    public List<Destination> Destinations { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? DestinationId { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    public async Task OnGetAsync()
    {
        Destinations = await _context.Destinations.ToListAsync();

        var query = _context.Packages
            .Include(p => p.Destination)
            .Include(p => p.Departures)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Category) && Enum.TryParse<PackageCategory>(Category, true, out var catEnum))
        {
            query = query.Where(p => p.Category == catEnum);
        }

        if (DestinationId.HasValue && DestinationId.Value > 0)
        {
            query = query.Where(p => p.DestinationId == DestinationId.Value);
        }

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var s = Search.Trim().ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(s) || 
                                     p.Summary.ToLower().Contains(s) || 
                                     p.Inclusions.ToLower().Contains(s));
        }

        if (MaxPrice.HasValue && MaxPrice.Value > 0)
        {
            query = query.Where(p => p.PricePerPerson <= MaxPrice.Value);
        }

        query = SortBy switch
        {
            "PriceAsc" => query.OrderBy(p => p.PricePerPerson),
            "PriceDesc" => query.OrderByDescending(p => p.PricePerPerson),
            "Rating" => query.OrderByDescending(p => p.Rating),
            "Duration" => query.OrderBy(p => p.DurationDays),
            _ => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.Rating)
        };

        Packages = await query.ToListAsync();
    }
}
