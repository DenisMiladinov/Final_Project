using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("VacationSpot")]
        public int SpotId { get; set; }
        public VacationSpot VacationSpot { get; set; }

        public int Rating { get; set; } // e.g., 1–5

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

