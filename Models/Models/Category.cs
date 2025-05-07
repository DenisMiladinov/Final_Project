using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<VacationSpot> VacationSpots { get; set; }
            = new List<VacationSpot>();

    }
}
