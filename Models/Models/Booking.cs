using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("VacationSpot")]
        public int SpotId { get; set; }
        public VacationSpot VacationSpot { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string StripeSessionId { get; set; }
        public bool IsCancelled { get; set; } = false;
        public DateTime CreatedAt { get; set; }
    }
}
