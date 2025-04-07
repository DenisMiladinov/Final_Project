using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<VacationSpot> VacationSpot { get; set; } = [];
}

