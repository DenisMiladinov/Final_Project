using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;
public class Image
{
    [Key]
    public int ImageId { get; set; }

    public string ImageUrl { get; set; }

    [ForeignKey("VacationSpot")]
    public int SpotId { get; set; }
    public VacationSpot VacationSpot { get; set; }
}