namespace Models;
public class Image
{
    public int ImageId { get; set; }

    public string ImageUrl { get; set; }

    public int SpotId { get; set; }
    public VacationSpot VacationSpot { get; set; }
}