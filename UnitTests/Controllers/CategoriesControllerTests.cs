using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Server.Controllers;

namespace UnitTests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            // Use a new in-memory DB for each test class instance
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new CategoriesController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAllCategories()
        {
            _context.Categories.AddRange(
                new Category { CategoryId = 1, Name = "Beach" },
                new Category { CategoryId = 2, Name = "Mountain" }
            );
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsCategory_WhenIdIsValid()
        {
            var cat = new Category { CategoryId = 5, Name = "Test" };
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();

            var result = await _controller.Details(5);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Category>(viewResult.Model);
            Assert.Equal(5, model.CategoryId);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = await _controller.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var result = await _controller.Details(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Post_RedirectsToIndex_WhenModelIsValid()
        {
            var newCategory = new Category { Name = "City" };

            var result = await _controller.Create(newCategory);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == "City");
            Assert.NotNull(category);
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var category = new Category();

            var result = await _controller.Create(category);

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<Category>(view.Model);
        }
    }
}
