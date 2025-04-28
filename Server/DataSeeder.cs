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

            string[] roles = { "Admin", "Receptionist", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@example.com";
            var adminPassword = "Admin123!";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    throw new Exception("Failed to create admin user: " +
                        string.Join("; ", createResult.Errors.Select(e => e.Description)));
                }
            }

            var adminRole = "Admin";
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }

            var userEmail = "user@example.com";
            var appUser = await userManager.FindByEmailAsync(userEmail);
            if (appUser == null)
            {
                appUser = new ApplicationUser
                {
                    UserName = "GuestUser",
                    Email = userEmail,
                    EmailConfirmed = true
                };
                var guestResult = await userManager.CreateAsync(appUser, "User123!");
                if (!guestResult.Succeeded)
                    throw new Exception("Failed to create guest user: " + string.Join("; ", guestResult.Errors.Select(e => e.Description)));
                await userManager.AddToRoleAsync(appUser, "User");
            }

            var recEmail = "receptionist@example.com";
            var recUser = await userManager.FindByEmailAsync(recEmail);
            if (recUser == null) 
            {
                recUser = new ApplicationUser
                {
                    UserName = "Receptionist",
                    Email = recEmail,
                    EmailConfirmed = true
                };
                var recResult = await userManager.CreateAsync(recUser, "Reception123!");
                if (!recResult.Succeeded)
                    throw new Exception("Failed to create receptionist: " +
                        string.Join("; ", recResult.Errors.Select(e => e.Description)));
            }
            if (!await userManager.IsInRoleAsync(recUser, "Receptionist"))
            {
                userManager.AddToRoleAsync(recUser, "Receptionist");
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new[]
                {
                new Category { Name = "Uncategorized" },
                new Category { Name = "Beach" },
                new Category { Name = "Mountain" },
                new Category { Name = "City" }
                });
                await context.SaveChangesAsync();
            }

            var defaultCat = await context.Categories.FirstAsync(c => c.Name == "Uncategorized");

            var spots = new List<VacationSpot>
            {
                new VacationSpot
                {
                    Title = "Beachside Villa",
                    Description = "Relax in a luxury villa right on the Black Sea coast.",
                    Location = "Sozopol, Bulgaria",
                    PricePerNight = 450.00M,
                    Images = new List<Image> { 
                        new Image { ImageUrl = "/assets/Spots/villa1.jpg" },
                        new Image { ImageUrl = "/assets/Spots/cabin1.jpg" },
                        new Image { ImageUrl = "/assets/Spots/loft1.jpg" }
                    },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Mountain Cabin",
                    Description = "A cozy cabin near the ski slopes of Bansko.",
                    Location = "Bansko, Bulgaria",
                    PricePerNight = 280.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/cabin1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Urban Loft",
                    Description = "Modern apartment in the center of Sofia.",
                    Location = "Sofia, Bulgaria",
                    PricePerNight = 320.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/loft1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Lakefront Cottage",
                    Description = "Quiet retreat with a view of Batak Lake.",
                    Location = "Batak, Bulgaria",
                    PricePerNight = 220.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/cottage1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Eco Bungalow",
                    Description = "Stay in a sustainable bungalow surrounded by forest.",
                    Location = "Velingrad, Bulgaria",
                    PricePerNight = 190.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/bungalow1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Desert Retreat",
                    Description = "Sunny and peaceful home in a quiet Rhodope village.",
                    Location = "Smolyan, Bulgaria",
                    PricePerNight = 210.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/retreat1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Historical Townhouse",
                    Description = "Vintage townhouse close to the old town.",
                    Location = "Plovdiv, Switzerland",
                    PricePerNight = 250.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/townhouse1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Cliffside Apartment",
                    Description = "Panoramic views of cliffs and the sea.",
                    Location = "Kavarna, Bulgaria",
                    PricePerNight = 300.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/apartment1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Countryside Farmhouse",
                    Description = "Rustic charm surrounded by meadows and hills.",
                    Location = "Veliko Tarnovo, Greece",
                    PricePerNight = 260.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/farmhouse1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Ski-In Chalet",
                    Description = "Hit the slopes at Pamporovo right from your door.",
                    Location = "Pamporovo, Bulgaria",
                    PricePerNight = 370.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/chalet1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Valley Cabin",
                    Description = "Explore the valleys and mountain terrain.",
                    Location = "Wasserwerk Freiberg, Germany",
                    PricePerNight = 450.00M,
                    Images = new List<Image> { 
                        new Image { ImageUrl = "/assets/Spots/valleycabin1.jpg" },
                        new Image { ImageUrl = "/assets/Spots/cabin1.jpg" },
                        new Image { ImageUrl = "/assets/Spots/loft1.jpg" }
                    },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Patagonia",
                    Description = "Adventure into a new mythical land.",
                    Location = "Patagonia, Argentina and Chile",
                    PricePerNight = 500.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/mountaincabin1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                },
                new VacationSpot
                {
                    Title = "Rural House",
                    Description = "Explore the village of Dolomites.",
                    Location = "The Dolomites, Italy",
                    PricePerNight = 350.00M,
                    Images = new List<Image> { new Image { ImageUrl = "/assets/Spots/ruralhouse1.jpg" } },
                    OwnerId = appUser.Id,
                    CategoryId = defaultCat.CategoryId
                }
            };

            foreach (var seedSpot in spots)
            {
                var existing = await context.VacationSpots
                    .Include(v => v.Images)
                    .FirstOrDefaultAsync(v => v.Title == seedSpot.Title);

                if (existing == null)
                {
                    context.VacationSpots.Add(seedSpot);
                }
                else
                {
                    existing.Description = seedSpot.Description;
                    existing.Location = seedSpot.Location;
                    existing.PricePerNight = seedSpot.PricePerNight;

                    existing.Images.Clear();
                    foreach (var img in seedSpot.Images)
                        existing.Images.Add(new Image { ImageUrl = img.ImageUrl });
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
