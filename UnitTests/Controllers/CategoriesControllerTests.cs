/*using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Server.Controllers;
using Xunit;

namespace UnitTests.Controllers
{
    public class CategoriesControllerTests
    {
        private ApplicationDbContext CreateContext(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithCategories()
        {
            var ctx = CreateContext(nameof(Index_ReturnsViewWithCategories));
            ctx.Categories.AddRange(
                new Category { CategoryId = 1, Name = "A" },
                new Category { CategoryId = 2, Name = "B" }
            );
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Index() as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(result.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Details_NullId_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Details(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Details_InvalidId_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Details(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithCategory()
        {
            var ctx = CreateContext(nameof(Details_ValidId_ReturnsViewWithCategory));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Details(1) as ViewResult;
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal("A", model.Name);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var ctx = CreateContext(nameof(Create_Get_ReturnsView));
            var ctrl = new CategoriesController(ctx);
            var result = ctrl.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var ctx = CreateContext(nameof(Create_Post_ValidModel_RedirectsToIndex));
            var ctrl = new CategoriesController(ctx);
            var cat = new Category { Name = "X" };
            var result = await ctrl.Create(cat) as RedirectToActionResult;
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(1, ctx.Categories.Count());
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            var ctx = CreateContext(nameof(Create_Post_InvalidModel_ReturnsViewWithModel));
            var ctrl = new CategoriesController(ctx);
            ctrl.ModelState.AddModelError("Name", "Required");
            var cat = new Category { Name = null };
            var result = await ctrl.Create(cat) as ViewResult;
            var model = Assert.IsType<Category>(result.Model);
            Assert.Null(model.Name);
        }

        [Fact]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Edit_Get_InvalidId_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Edit(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ValidId_ReturnsViewWithCategory()
        {
            var ctx = CreateContext(nameof(Edit_Get_ValidId_ReturnsViewWithCategory));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Edit(1) as ViewResult;
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal("A", model.Name);
        }

        [Fact]
        public async Task Edit_Post_IdMismatch_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Edit_Post_IdMismatch_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var cat = new Category { CategoryId = 2, Name = "X" };
            var result = await ctrl.Edit(1, cat);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            var ctx = CreateContext(nameof(Edit_Post_InvalidModel_ReturnsViewWithModel));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            ctrl.ModelState.AddModelError("Name", "Required");
            var cat = new Category { CategoryId = 1, Name = null };
            var result = await ctrl.Edit(1, cat) as ViewResult;
            var model = Assert.IsType<Category>(result.Model);
            Assert.Null(model.Name);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            var ctx = CreateContext(nameof(Edit_Post_ValidModel_RedirectsToIndex));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var cat = new Category { CategoryId = 1, Name = "B" };
            var result = await ctrl.Edit(1, cat) as RedirectToActionResult;
            Assert.Equal("Index", result.ActionName);
            var updated = await ctx.Categories.FindAsync(1);
            Assert.Equal("B", updated.Name);
        }

        [Fact]
        public async Task Delete_Get_NullId_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Delete_Get_NullId_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Delete(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            var ctx = CreateContext(nameof(Delete_Get_InvalidId_ReturnsNotFound));
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Delete(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Get_ValidId_ReturnsViewWithCategory()
        {
            var ctx = CreateContext(nameof(Delete_Get_ValidId_ReturnsViewWithCategory));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.Delete(1) as ViewResult;
            var model = Assert.IsType<Category>(result.Model);
            Assert.Equal("A", model.Name);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesAndRedirects()
        {
            var ctx = CreateContext(nameof(DeleteConfirmed_RemovesAndRedirects));
            ctx.Categories.Add(new Category { CategoryId = 1, Name = "A" });
            await ctx.SaveChangesAsync();
            var ctrl = new CategoriesController(ctx);
            var result = await ctrl.DeleteConfirmed(1) as RedirectToActionResult;
            Assert.Equal("Index", result.ActionName);
            Assert.Null(await ctx.Categories.FindAsync(1));
        }
    }
}*/