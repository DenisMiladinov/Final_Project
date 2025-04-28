namespace Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public int TotalSpots { get; set; }
        public List<Category> Categories { get; set; } = new();
        public int TotalCategories { get; set; }
    }
}
