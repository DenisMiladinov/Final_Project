using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models.ViewModels
{
    public class VacationSpotFormViewModel
    {
        public int? SpotId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? Location { get; set; }

        [Required, Range(0.01, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Categories")]
        [Required(ErrorMessage = "Pick at least one category")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        public MultiSelectList CategoriesList { get; set; } = new MultiSelectList(new List<SelectListItem>());
    }
}
