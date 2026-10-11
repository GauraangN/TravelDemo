using System.ComponentModel.DataAnnotations;

namespace TravelDemo.Models;

public enum PackageCategory { Adventure, Beach, Culture, Luxury, Family, Wellness }

public class TravelPackage
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Title { get; set; } = "";
    public int DestinationId { get; set; }
    public Destination? Destination { get; set; }
    public string Summary { get; set; } = "";
    public int DurationDays { get; set; }
    [DataType(DataType.Currency)] public decimal PricePerPerson { get; set; }
    public int MaxGroupSize { get; set; } = 12;
    public PackageCategory Category { get; set; }
    public double Rating { get; set; }
    public bool IsFeatured { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Inclusions { get; set; } = ""; // comma-separated for the demo
    public List<ItineraryDay> Itinerary { get; set; } = new();
    public List<Departure> Departures { get; set; } = new();
}

public class ItineraryDay
{
    public int Id { get; set; }
    public int TravelPackageId { get; set; }
    public int DayNumber { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}
