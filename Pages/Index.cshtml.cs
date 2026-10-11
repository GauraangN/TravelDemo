using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Data;
using TravelDemo.Models;

namespace TravelDemo.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Destination> Destinations { get; set; } = new();
    public List<TravelPackage> FeaturedPackages { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Category { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    public async Task OnGetAsync()
    {
        Destinations = await _context.Destinations
            .Include(d => d.Packages)
            .Take(6)
            .ToListAsync();

        var query = _context.Packages
            .Include(p => p.Destination)
            .Include(p => p.Departures)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Category) && Enum.TryParse<PackageCategory>(Category, true, out var catEnum))
        {
            query = query.Where(p => p.Category == catEnum);
        }

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var s = Search.Trim().ToLower();
            query = query.Where(p => p.Title.ToLower().Contains(s) || 
                                     p.Summary.ToLower().Contains(s) || 
                                     p.Destination!.Name.ToLower().Contains(s) || 
                                     p.Destination.Country.ToLower().Contains(s));
        }

        if (MaxPrice.HasValue && MaxPrice.Value > 0)
        {
            query = query.Where(p => p.PricePerPerson <= MaxPrice.Value);
        }

        FeaturedPackages = await query.ToListAsync();
    }
}
