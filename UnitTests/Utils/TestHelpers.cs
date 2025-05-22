using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class TestHelpers
    {
        public static ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(opts);
        }

        public static UserManager<ApplicationUser> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            ).Object;
        }

        public static ControllerContext GetControllerContextWithUser(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        /// <summary>
        /// Wraps a List<T> so that EF Core async LINQ (CountAsync, ToListAsync) will work.
        /// </summary>
        public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

            public IQueryable CreateQuery(Expression expression) =>
                new TestAsyncEnumerable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
                new TestAsyncEnumerable<TElement>(expression);

            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) =>
                _inner.Execute<TResult>(expression);

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken _)
            {
                var resultType = typeof(TResult).GetGenericArguments()[0];
                var exec = typeof(IQueryProvider)
                    .GetMethod(nameof(IQueryProvider.Execute), new[] { typeof(Expression) })
                    .MakeGenericMethod(resultType)
                    .Invoke(_inner, new[] { expression });
                return (TResult)typeof(Task)
                    .GetMethod(nameof(Task.FromResult))
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { exec });
            }
        }

        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken ct = default) =>
                new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            IQueryProvider IQueryable.Provider =>
                new TestAsyncQueryProvider<T>(this);
        }

        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return default; }
            public ValueTask<bool> MoveNextAsync() =>
                new ValueTask<bool>(_inner.MoveNext());
        }

        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source) =>
            new TestAsyncEnumerable<T>(source);
    }
}
