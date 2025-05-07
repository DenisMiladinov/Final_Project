using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class ReviewViewModel
    {
        public int SpotId { get; set; }

        [Required(ErrorMessage = "Please choose a rating")]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please leave a comment")]
        public string Comment { get; set; }
    }

}
