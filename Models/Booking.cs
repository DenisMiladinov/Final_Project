namespace Models;

public class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; }

    public int SpotId { get; set; }
    public VacationSpot VacationSpot { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal TotalPrice { get; set; }

    public Payment Payment { get; set; }
}
