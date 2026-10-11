namespace TravelDemo.Models;

public class Destination
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public string Region { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public List<TravelPackage> Packages { get; set; } = new();
}
