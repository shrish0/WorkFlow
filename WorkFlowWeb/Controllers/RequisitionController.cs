using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Models;
using WorkFlow.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkFlow.Data.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace WorkFlowWeb.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequisitionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Index Page
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.RequisitionBodies.ToListAsync());
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
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve the current user's ClearanceLevel
            var currentClearanceLevel = await _context.Users
                                                      .Where(u => u.Id == currentUserId)
                                                      .Select(u => u.ClearanceLevel)
                                                      .FirstOrDefaultAsync();

            string incrementedClearanceLevel = null;

            if (!string.IsNullOrEmpty(currentClearanceLevel) && currentClearanceLevel.StartsWith("Cl"))
            {
                // Extract the numeric part of the ClearanceLevel
                var numericPart = int.Parse(currentClearanceLevel.Substring(2));

                // Increment the numeric part by 1
                numericPart++;

                // Combine the prefix "Cl" with the incremented number, padded to two digits
                incrementedClearanceLevel = "Cl" + numericPart.ToString("D2");
            }

            // Select the email addresses of users with the incremented ClearanceLevel
            var usersEmail = await _context.Users
                                           .Where(u => u.ClearanceLevel == incrementedClearanceLevel)
                                           .Select(u => u.Email)
                                           .ToListAsync();

            var viewModel = new RequisitionViewModel
            {
                UsersEmail = usersEmail,
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequisitionViewModel viewModel)
        {
            // Remove unnecessary validation for Categories and SubCategories
            ModelState.Remove(nameof(viewModel.RequisitionHeader.RequisitionId));
            ModelState.Remove(nameof(viewModel.RequisitionBody.RequisitionId));
            ModelState.Remove(nameof(viewModel.Categories));
            ModelState.Remove(nameof(viewModel.SubCategories));

            if (ModelState.IsValid)
            {
                // Generate RequisitionId
                viewModel.RequisitionHeader.RequisitionId = GenerateRequisitionId();
                viewModel.RequisitionBody.RequisitionId = viewModel.RequisitionHeader.RequisitionId;

                var currentUserId = _userManager.GetUserId(User);

                // Retrieve the sender and receiver UserIds
                var sentTo = await _context.Users.Where(u => u.Email == viewModel.RequisitionApproval.SentTo)
                                                  .Select(u => u.ApplicationUserId)
                                                  .FirstOrDefaultAsync();
                var sentBy = await _context.Users.Where(u => u.Id == currentUserId)
                                                  .Select(u => u.ApplicationUserId)
                                                  .FirstOrDefaultAsync();

                viewModel.RequisitionHeader.CreatedBy = sentBy;
                viewModel.RequisitionHeader.CreatedAt = DateTime.Now;
                viewModel.RequisitionApproval.SentTo = sentTo;
                viewModel.RequisitionApproval.SentBy = sentBy;

                // First save RequisitionHeader and RequisitionBody to ensure RequisitionId exists
                _context.Add(viewModel.RequisitionHeader);
                _context.Add(viewModel.RequisitionBody);
                await _context.SaveChangesAsync();

                // Now save RequisitionApproval with the correct RequisitionId
                viewModel.RequisitionApproval.RequisitionId = viewModel.RequisitionHeader.RequisitionId;
                _context.Add(viewModel.RequisitionApproval);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload categories and subcategories in case of validation error
            viewModel.Categories = await _context.Categories.ToListAsync();
            viewModel.SubCategories = await _context.SubCategories.ToListAsync();
            viewModel.UsersEmail = await _context.Users.Select(u => u.Email).ToListAsync();
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
                    .Where(r => r.RequisitionId.StartsWith($"REQ{currentYear}"))
                    .OrderByDescending(r => r.RequisitionId)
                    .FirstOrDefault();

                int lastNumber = 0;

                if (lastRequisition != null && !string.IsNullOrEmpty(lastRequisition.RequisitionId))
                {
                    // Extract the numeric part after "REQ" + year
                    string numericPart = lastRequisition.RequisitionId.Substring(7); // Length of "REQ" + year (3 + 4 = 7)
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
