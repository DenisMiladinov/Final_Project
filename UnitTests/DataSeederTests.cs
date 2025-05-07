// File: UnitTests/DataSeederTests.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Models;
using Server;
using Xunit;

namespace UnitTests
{
    public class DataSeederTests
    {
        private ServiceProvider BuildServiceProvider(string dbName)
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseInMemoryDatabase(dbName));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task SeedAsync_CreatesRolesUsersCategoriesSpotsAndBookings()
        {
            var provider = BuildServiceProvider(nameof(SeedAsync_CreatesRolesUsersCategoriesSpotsAndBookings));
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

            await DataSeeder.SeedAsync(context, userManager, roleManager, userStore);

            Assert.Equal(3, context.Roles.Count());
            Assert.Contains(context.Roles, r => r.Name == "Admin");
            Assert.Contains(context.Roles, r => r.Name == "User");
            Assert.Contains(context.Roles, r => r.Name == "Receptionist");

            Assert.Equal(3, context.Users.Count());
            Assert.Contains(context.Users, u => u.Email == "admin@example.com");
            Assert.Contains(context.Users, u => u.Email == "user@example.com");
            Assert.Contains(context.Users, u => u.Email == "receptionist@example.com");

            Assert.Equal(4, context.Categories.Count());
            Assert.Contains(context.Categories, c => c.Name == "Uncategorized");
            Assert.Contains(context.Categories, c => c.Name == "Beach");
            Assert.Contains(context.Categories, c => c.Name == "Mountain");
            Assert.Contains(context.Categories, c => c.Name == "City");

            Assert.True(context.VacationSpots.Any());
            Assert.True(context.Bookings.Any());
        }

        [Fact]
        public async Task SeedAsync_IsIdempotent()
        {
            var provider = BuildServiceProvider(nameof(SeedAsync_IsIdempotent));
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<ApplicationUser>>();

            await DataSeeder.SeedAsync(context, userManager, roleManager, userStore);
            var roleCount = context.Roles.Count();
            var userCount = context.Users.Count();
            var catCount = context.Categories.Count();
            var spotCount = context.VacationSpots.Count();
            var bookingCount = context.Bookings.Count();

            await DataSeeder.SeedAsync(context, userManager, roleManager, userStore);

            Assert.Equal(roleCount, context.Roles.Count());
            Assert.Equal(userCount, context.Users.Count());
            Assert.Equal(catCount, context.Categories.Count());
            Assert.Equal(spotCount, context.VacationSpots.Count());
            Assert.Equal(bookingCount, context.Bookings.Count());
        }
    }
}
