namespace Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int SpotId { get; set; }
        public VacationSpot VacationSpot { get; set; }

        public int Rating { get; set; } // e.g., 1–5

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
