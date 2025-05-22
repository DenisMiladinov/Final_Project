using System.Collections.Generic;
using Models;                // for VacationSpot & Review
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels
{
    public class VacationSpotDetailsViewModel
    {
        public VacationSpot Spot { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public ReviewViewModel NewReview { get; set; }
    }
}
