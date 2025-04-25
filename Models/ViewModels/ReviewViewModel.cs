using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class ReviewViewModel
    {
        public int SpotId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
