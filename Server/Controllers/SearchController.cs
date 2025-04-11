using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace Server.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string location, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            var results = await _searchService.SearchAsync(location, minPrice, maxPrice, startDate, endDate);
            return View(results);
        }
    }
}
