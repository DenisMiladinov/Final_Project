using System.ComponentModel.DataAnnotations;

namespace Models;

public class VacationSpot
{
    public int SpotId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public string Location { get; set; }

    public decimal PricePerNight { get; set; }

    public int? OwnerId { get; set; }
    public ApplicationUser Owner { get; set; } // Optional link to the user/admin who added it

    public ICollection<Image> Images { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
