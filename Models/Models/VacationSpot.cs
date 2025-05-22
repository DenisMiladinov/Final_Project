using Models.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class VacationSpot
    {
        [Key]
        public int SpotId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public string? Location { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [Required]
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<Image> Images { get; set; } = new List<Image>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<VacationSpotCategory> VacationSpotCategories { get; set; }
    }
}
