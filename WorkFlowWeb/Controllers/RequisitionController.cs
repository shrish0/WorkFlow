using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Models;
using WorkFlow.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkFlow.Data.DataAccess;

namespace WorkFlowWeb.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequisitionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index Page
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // Detail
        public async Task<IActionResult> Detail(string id)
        {
            var requisitionHeader = await _context.RequisitionHeaders
                .Include(r => r.Category)
                .Include(r => r.SubCategory)
                .FirstOrDefaultAsync(r => r.RequisitionId == id);

            if (requisitionHeader == null)
            {
                return NotFound();
            }

            var requisitionBody = await _context.RequisitionBodies
                .FirstOrDefaultAsync(rb => rb.RequisitionId == id);

            var viewModel = new RequisitionViewModel
            {
                RequisitionHeader = requisitionHeader,
                RequisitionBody = requisitionBody,
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.ToListAsync()
            };

            return View(viewModel);
        }

        // GET: Requisition/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new RequisitionViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.ToListAsync(),
                RequisitionHeader = new RequisitionHeader(),
                RequisitionBody = new RequisitionBody
                {
                    HasAttachment = false // Default value
                }
            };

            return View(viewModel);
        }

        // POST: Requisition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequisitionViewModel viewModel)
        {
            

            // Remove unnecessary validation for Categories and SubCategories
            // Remove validation for RequisitionId and other unnecessary fields
            ModelState.Remove(nameof(viewModel.RequisitionHeader.RequisitionId));
            ModelState.Remove(nameof(viewModel.RequisitionBody.RequisitionId));
            ModelState.Remove(nameof(viewModel.Categories));
            ModelState.Remove(nameof(viewModel.SubCategories));

            viewModel.RequisitionHeader.RequisitionId = GenerateRequisitionId();
            viewModel.RequisitionBody.RequisitionId = viewModel.RequisitionHeader.RequisitionId;

            if (ModelState.IsValid)
            {
                viewModel.RequisitionHeader.CreatedBy = User.Identity.Name;
                viewModel.RequisitionHeader.CreatedAt = DateTime.Now;

                _context.Add(viewModel.RequisitionHeader);
                _context.Add(viewModel.RequisitionBody);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload categories and subcategories in case of validation error
            viewModel.Categories = await _context.Categories.ToListAsync();
            viewModel.SubCategories = await _context.SubCategories.ToListAsync();

            return View(viewModel);
        }

        private string GenerateRequisitionId()
        {
            try
            {
                // Get the current year
                string currentYear = DateTime.Now.Year.ToString();

                // Query the database for the latest requisition ID for the current year
                var lastRequisition = _context.RequisitionHeaders
                    .Where(r => r.RequisitionId.StartsWith(currentYear))
                    .OrderByDescending(r => r.RequisitionId)
                    .FirstOrDefault();

                int lastNumber = 0;

                if (lastRequisition != null && !string.IsNullOrEmpty(lastRequisition.RequisitionId))
                {
                    // Extract the numeric part after the year
                    string numericPart = lastRequisition.RequisitionId.Substring(4); // Length of "REQ" + year
                    if (int.TryParse(numericPart, out int parsedNumber))
                    {
                        lastNumber = parsedNumber;
                    }
                }

                int newNumber = lastNumber + 1;
                return $"REQ{currentYear}{newNumber:D6}"; // Format new number with zero-padding to 6 digits
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new InvalidOperationException("Error generating Requisition ID.", ex);
            }
        }
    }
}
