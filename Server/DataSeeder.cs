using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Server
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<ApplicationUser> userStore)
        {
            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (await userManager.FindByEmailAsync("user@example.com") == null)
            {
                var user = new ApplicationUser();
                await userStore.SetUserNameAsync(user, "user@example.com", default);
                await ((IUserEmailStore<ApplicationUser>)userStore)
                      .SetEmailAsync(user, "user@example.com", default);
                await ((IUserEmailStore<ApplicationUser>)userStore)
                      .SetEmailConfirmedAsync(user, true, default);
                await userManager.CreateAsync(user, "User123!");
                await userManager.AddToRoleAsync(user, "User");
            }

            var appUser = await userManager.FindByEmailAsync("user@example.com");
            if (appUser == null) throw new Exception("User not found");

            var spots = new List<VacationSpot>
            {
                new VacationSpot
                {
                    Title = "Beachside Villa",
                    Description = "Relax in a luxury villa right on the Black Sea coast.",
                    Location = "Sozopol, Bulgaria",
                    PricePerNight = 450.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/villa1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Mountain Cabin",
                    Description = "A cozy cabin near the ski slopes of Bansko.",
                    Location = "Bansko, Bulgaria",
                    PricePerNight = 280.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/cabin1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Urban Loft",
                    Description = "Modern apartment in the center of Sofia.",
                    Location = "Sofia, Bulgaria",
                    PricePerNight = 320.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/loft1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Lakefront Cottage",
                    Description = "Quiet retreat with a view of Batak Lake.",
                    Location = "Batak, Bulgaria",
                    PricePerNight = 220.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/cottage1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Eco Bungalow",
                    Description = "Stay in a sustainable bungalow surrounded by forest.",
                    Location = "Velingrad, Bulgaria",
                    PricePerNight = 190.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/bungalow1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Desert-Inspired Retreat",
                    Description = "Sunny and peaceful home in a quiet Rhodope village.",
                    Location = "Smolyan, Bulgaria",
                    PricePerNight = 210.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/retreat1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Historical Townhouse",
                    Description = "Vintage townhouse close to the old town.",
                    Location = "Plovdiv, Switzerland",
                    PricePerNight = 250.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/townhouse1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Cliffside Apartment",
                    Description = "Panoramic views of cliffs and the sea.",
                    Location = "Kavarna, Bulgaria",
                    PricePerNight = 300.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/apartment1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Countryside Farmhouse",
                    Description = "Rustic charm surrounded by meadows and hills.",
                    Location = "Veliko Tarnovo, Greece",
                    PricePerNight = 260.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/farmhouse1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Ski-In Chalet",
                    Description = "Hit the slopes at Pamporovo right from your door.",
                    Location = "Pamporovo, Bulgaria",
                    PricePerNight = 370.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/chalet1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Valley Cabin",
                    Description = "Explore the valleys and mountain terrain.",
                    Location = "Wasserwerk Freiberg, Germany",
                    PricePerNight = 350.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/valleycabin1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Patagonia",
                    Description = "Adventure into a new mythical land.",
                    Location = "Patagonia, Argentina and Chile",
                    PricePerNight = 500.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/mountaincabin1.jpg" } },
                    OwnerId = appUser.Id
                },
                new VacationSpot
                {
                    Title = "Rural House",
                    Description = "Explore the village of Dolomites.",
                    Location = "The Dolomites, Italy",
                    PricePerNight = 4500.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/ruralhouse1.jpg" } },
                    OwnerId = appUser.Id
                }
            };

            foreach (var spot in spots)
            {
                if (!await context.VacationSpots.AnyAsync(v => v.Title == spot.Title))
                {
                    context.VacationSpots.Add(spot);
                }
            }

            await context.SaveChangesAsync();

            if (!context.Bookings.Any())
            {
                var firstSpot = await context.VacationSpots.FirstOrDefaultAsync();
                if (firstSpot != null)
                {
                    var booking = new Booking
                    {
                        UserId = appUser.Id,
                        SpotId = firstSpot.SpotId,
                        StartDate = DateTime.Today.AddDays(7),
                        EndDate = DateTime.Today.AddDays(10),
                        TotalPrice = 3 * firstSpot.PricePerNight
                    };
                    context.Bookings.Add(booking);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
