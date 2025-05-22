/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Services;
using Xunit;

namespace UnitTests.Services
{
    public class VacationSpotServiceTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(opts);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSpotsWithRelated()
        {
            var ctx = CreateContext(nameof(GetAllAsync_ReturnsAllSpotsWithRelated));
            var cat = new Category { CategoryId = 1, Name = "TestCat" };
            ctx.Categories.Add(cat);
            ctx.VacationSpots.Add(new VacationSpot
            {
                SpotId = 1,
                Title = "A",
                CategoryId = cat.CategoryId,
                Images = new List<Image> { new Image { ImageId = 1, ImageUrl = "/i.jpg", SpotId = 1 } }
            });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            var all = await svc.GetAllAsync();

            var spot = Assert.Single(all);
            Assert.Equal("A", spot.Title);
            Assert.NotNull(spot.Category);
            Assert.Single(spot.Images);
        }

        [Fact]
        public async Task GetByIdAsync_FindsSpotOrNull()
        {
            var ctx = CreateContext(nameof(GetByIdAsync_FindsSpotOrNull));
            ctx.VacationSpots.Add(new VacationSpot { SpotId = 10, Title = "Ten" });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            var found = await svc.GetByIdAsync(10);
            Assert.NotNull(found);
            Assert.Equal("Ten", found.Title);

            var missing = await svc.GetByIdAsync(11);
            Assert.Null(missing);
        }

        [Fact]
        public async Task CreateAsync_AddsSpot()
        {
            var ctx = CreateContext(nameof(CreateAsync_AddsSpot));
            var svc = new VacationSpotService(ctx);

            var spot = new VacationSpot { SpotId = 5, Title = "New" };
            await svc.CreateAsync(spot);

            var fromDb = await ctx.VacationSpots.FindAsync(5);
            Assert.NotNull(fromDb);
            Assert.Equal("New", fromDb.Title);
        }

        [Fact]
        public async Task UpdateAsync_ChangesSpot()
        {
            var ctx = CreateContext(nameof(UpdateAsync_ChangesSpot));
            ctx.VacationSpots.Add(new VacationSpot { SpotId = 2, Title = "Old" });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            var existing = new VacationSpot { SpotId = 2, Title = "Updated" };
            await svc.UpdateAsync(existing);

            var fromDb = await ctx.VacationSpots.FindAsync(2);
            Assert.Equal("Updated", fromDb.Title);
        }

        [Fact]
        public async Task DeleteAsync_RemovesSpotIfExists()
        {
            var ctx = CreateContext(nameof(DeleteAsync_RemovesSpotIfExists));
            ctx.VacationSpots.Add(new VacationSpot { SpotId = 3, Title = "ToDelete" });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            await svc.DeleteAsync(3);

            Assert.Null(await ctx.VacationSpots.FindAsync(3));
        }

        [Fact]
        public async Task GetByLocationAsync_FiltersBySubstring()
        {
            var ctx = CreateContext(nameof(GetByLocationAsync_FiltersBySubstring));
            ctx.VacationSpots.AddRange(
                new VacationSpot { SpotId = 1, Location = "Beachside" },
                new VacationSpot { SpotId = 2, Location = "Mountain" }
            );
            ctx.Images.Add(new Image { ImageId = 1, SpotId = 1, ImageUrl = "/i.jpg" });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            var result = await svc.GetByLocationAsync("Beach");

            var spot = Assert.Single(result);
            Assert.Equal(1, spot.SpotId);
            Assert.Single(spot.Images);
        }

        [Fact]
        public async Task GetAvailableSpotsAsync_ExcludesBooked()
        {
            var ctx = CreateContext(nameof(GetAvailableSpotsAsync_ExcludesBooked));
            ctx.VacationSpots.AddRange(
                new VacationSpot { SpotId = 1, Title = "A" },
                new VacationSpot { SpotId = 2, Title = "B" }
            );
            ctx.Bookings.Add(new Booking
            {
                BookingId = 1,
                SpotId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            });
            await ctx.SaveChangesAsync();

            var svc = new VacationSpotService(ctx);
            var avail = await svc.GetAvailableSpotsAsync(
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(3));

            Assert.Single(avail);
            Assert.Equal(2, avail.First().SpotId);
        }
    }
}*/