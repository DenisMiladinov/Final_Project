using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class BookingViewModel
    {
        public int SpotId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, 20)]
        public int GuestsCount { get; set; }
    }
}
