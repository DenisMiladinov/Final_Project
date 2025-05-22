using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;

namespace UnitTests
{
    public static class TestHelpers
    {
        public static UserManager<ApplicationUser> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            return mgr.Object;
        }

        public static ClaimsPrincipal GetMockUser(string userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
        }

        public static ControllerContext GetMockControllerContext(string userId)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = GetMockUser(userId)
                }
            };
        }

        public static ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
