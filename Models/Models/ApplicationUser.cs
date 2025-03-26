using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models;

public class ApplicationUser : IdentityUser
{

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; } // Store hashed passwords

    [Required]
    public string Role { get; set; } // e.g. "User", "Admin"

    public ICollection<Booking> Bookings { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<VacationSpot> VacationSpot { get; set; }
}

