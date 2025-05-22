using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Services.Services;
using Server.Controllers;
using UnitTests;

namespace UnitTests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IVacationSpotService> _spotServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ApplicationDbContext _context;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _spotServiceMock = new Mock<IVacationSpotService>();
            _bookingServiceMock = new Mock<IBookingService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            // unique in-memory DB per test class instance
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _controller = new AdminController(
                _spotServiceMock.Object,
                _bookingServiceMock.Object,
                _userManagerMock.Object,
                _context,
                env: null
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithCorrectViewModel()
        {
            // Arrange services
            _spotServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(new[] { new VacationSpot(), new VacationSpot() });

            _bookingServiceMock
                .Setup(b => b.GetAllAsync())
                .ReturnsAsync(new[] { new Booking() });

            // Seed real users into the EF InMemory context
            _context.Users.AddRange(
                new ApplicationUser(),
                new ApplicationUser(),
                new ApplicationUser()
            );
            await _context.SaveChangesAsync();

            // Have UserManager.Users return the real DbSet (which supports async)
            _userManagerMock
                .Setup(u => u.Users)
                .Returns(_context.Users);

            // Seed some categories too
            _context.Categories.AddRange(
                new Category { CategoryId = 1, Name = "A" },
                new Category { CategoryId = 2, Name = "B" },
                new Category { CategoryId = 3, Name = "C" }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<Models.ViewModels.AdminDashboardViewModel>(viewResult.Model);

            Assert.Equal(2, vm.TotalSpots);
            Assert.Equal(1, vm.TotalBookings);
            Assert.Equal(3, vm.TotalUsers);
            Assert.Equal(3, vm.TotalCategories);
        }
    }
}
