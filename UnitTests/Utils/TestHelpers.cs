// File: UnitTests/Utils/TestHelpers.cs
/*using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Utils
{
    public static class TestHelpers
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object,
                null,
                null,
                new IUserValidator<TUser>[0],
                new IPasswordValidator<TUser>[0],
                null,
                null,
                null,
                null
            );
        }

        public static ControllerContext GetControllerContext(string userId, params string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = user };

            return new ControllerContext
            {
                HttpContext = httpContext
            };
        }
    }
}*/