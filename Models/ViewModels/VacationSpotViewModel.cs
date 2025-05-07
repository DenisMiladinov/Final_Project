using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class VacationSpotViewModel
    {
        public int SpotId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Location { get; set; }

        public string? Description { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
}
