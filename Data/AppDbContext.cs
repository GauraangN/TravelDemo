using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelDemo.Models;

namespace TravelDemo.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Destination> Destinations => Set<Destination>();
    public DbSet<TravelPackage> Packages => Set<TravelPackage>();
    public DbSet<ItineraryDay> ItineraryDays => Set<ItineraryDay>();
    public DbSet<Departure> Departures => Set<Departure>();
    public DbSet<TravelBooking> Bookings => Set<TravelBooking>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        // SQLite can't sort/compare decimals natively; store money as double (demo only).
        b.Entity<TravelPackage>().Property(p => p.PricePerPerson).HasConversion<double>();
        b.Entity<TravelBooking>().Property(x => x.TotalPrice).HasConversion<double>();
        b.Entity<TravelPackage>().Property(p => p.Category).HasConversion<string>();
        b.Entity<TravelBooking>().Property(x => x.Status).HasConversion<string>();
        b.Entity<TravelBooking>().HasOne(x => x.Departure).WithMany().HasForeignKey(x => x.DepartureId).OnDelete(DeleteBehavior.Restrict);
    }
}
