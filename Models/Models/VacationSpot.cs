using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class VacationSpot
    {
        [Key]
        public int SpotId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public decimal PricePerNight { get; set; }

        [ForeignKey("Owner")]
        public string OwnerId { get; set; }

        public ApplicationUser Owner { get; set; }

        public ICollection<Image> Images { get; set; }

        public ICollection<Booking> Bookings { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}

