using System.ComponentModel.DataAnnotations;

namespace TravelDemo.Models;

public class Departure
{
    public int Id { get; set; }
    public int TravelPackageId { get; set; }
    public TravelPackage? TravelPackage { get; set; }
    public DateOnly StartDate { get; set; }
    public int SeatsTotal { get; set; }
    [ConcurrencyCheck]
    public int SeatsBooked { get; set; }
    public int SeatsLeft => SeatsTotal - SeatsBooked;
}

public enum BookingStatus { Pending, Confirmed, Cancelled }

public class TravelBooking
{
    public int Id { get; set; }
    public int DepartureId { get; set; }
    public Departure? Departure { get; set; }
    public string UserId { get; set; } = "";
    [Range(1, 20)] public int Travellers { get; set; } = 1;
    public decimal TotalPrice { get; set; }
    public string LeadName { get; set; } = "";
    public string Phone { get; set; } = "";
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
