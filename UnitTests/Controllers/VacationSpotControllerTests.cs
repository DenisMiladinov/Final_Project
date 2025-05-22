using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Services;
using Server.Controllers;

namespace UnitTests.Controllers
{
    public class VacationSpotControllerTests
    {
        private readonly Mock<IVacationSpotService> _vacationSpotServiceMock;
        private readonly ApplicationDbContext _context;
        private readonly VacationSpotController _controller;

        public VacationSpotControllerTests()
        {
            // 1) Unique in-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _vacationSpotServiceMock = new Mock<IVacationSpotService>();

            // 2) Inject real context, mocks for others
            _controller = new VacationSpotController(
                _vacationSpotServiceMock.Object,
                webHostEnvironment: null,
                context: _context,
                reviewService: null
            );
        }
    }
}
