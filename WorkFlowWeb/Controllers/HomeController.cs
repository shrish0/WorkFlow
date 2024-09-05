using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WorkFlow.Data.DataAccess;
using WorkFlow.Models;
using WorkFlow.ViewModels;

namespace WorkFlowWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CategoryIndex()
        {
            var category = await _db.Categories.ToListAsync();
            return View(category);
        }

        public async Task<IActionResult> SubCategoryIndex()
        {
            var subCategory = await _db.SubCategories
                            .Include(sc => sc.Category) // Include the Category entity
                            .Where(sc => sc.Category.IsActive) // Filter for active categories
                            .Select(sc => new SubCategoryViewModel
                            {
                                SubCategoryId = sc.SubCategoryId,
                                Code = sc.Code,
                                Description = sc.Description,
                                IsActive = sc.IsActive,
                                InactivatedBy = sc.InactivatedBy,
                                CategoryId = sc.CategoryId,
                                CategoryCode = sc.Category.Code // Include Category code
                            })
                            .ToListAsync();
            return View(subCategory);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
