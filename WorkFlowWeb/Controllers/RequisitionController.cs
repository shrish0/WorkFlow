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
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

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

            var currentUserId = _userManager.GetUserId(User);

            // Retrieve the current user's ClearanceLevel
            var applicationUserID = await _context.Users
                                                      .Where(u => u.Id == currentUserId)
                                                      .Select(u => u.ApplicationUserId)
                                                      .FirstOrDefaultAsync();

            var requisitions = await _context.RequisitionBodies
                    .Where(r => _context.RequisitionApprovals
                        .Any(a => (a.RequisitionId == r.RequisitionId) &&
                                  (a.SentTo == applicationUserID || a.SentBy == applicationUserID)))
                    .ToListAsync();
            

            return View(requisitions);
        }

        // Detail
        public async Task<IActionResult> Detail(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve the current user's ClearanceLevel
            var applicationUserID = await _context.Users
                                                      .Where(u => u.Id == currentUserId)
                                                      .Select(u => u.ApplicationUserId)
                                                      .FirstOrDefaultAsync();

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
            
            var requisitionSupplement = await _context.RequisitionSupplements
               .Where(rb => rb.RequisitionId == id).ToListAsync();

            var requisitionApproval = await _context.RequisitionApprovals
              .Where(ra => ra.RequisitionId == id && (ra.SentTo == applicationUserID || ra.SentBy == applicationUserID)).ToListAsync();
            


            var viewModel = new RequisitionDetailViewModels
            {
                RequisitionHeader = requisitionHeader,
                RequisitionBody = requisitionBody,
                RequisitionApproval= requisitionApproval,
                RequisitionSupplement= requisitionSupplement,
                ApplicationUserId=applicationUserID
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
                incrementedClearanceLevel = "CL" + numericPart.ToString("D2");
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

        // GET: Requisition/TakeAction
        public async Task<IActionResult> TakeAction(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
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
            // Retrieve the approval record based on the approval ID
            var approval = await _context.RequisitionApprovals
                .FirstOrDefaultAsync(a => a.ApprovalId == id);

            if (approval == null)
            {
                return NotFound();
            }

            // Prepare the ViewModel for the TakeAction view
            var viewModel = new RequisitionApprovalActionViewModel
            {
                ApprovalId = approval.ApprovalId,
                SentTo = approval.SentTo,
                Comment = approval.Comment,
                NewAction = approval.Action ,// Pre-select the current action
                UsersEmail = usersEmail,
            };

            // Return the view with the ViewModel
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeAction(RequisitionApprovalActionViewModel model)
        {
            var currentUserId = _userManager.GetUserId(User);
            var applicationUserId = _context.ApplicationUsers.Where(u=>(u.Id==currentUserId))
                                .Select(u=>u.ApplicationUserId)
                                .FirstOrDefault();
            if (ModelState.IsValid)
            {
                var approval = await _context.RequisitionApprovals
                    .FirstOrDefaultAsync(a => a.ApprovalId == model.ApprovalId);

                if (approval == null)
                {
                    return NotFound();
                }

                if (model.NewAction == RequisitionAction.SuccessFull || model.NewAction == RequisitionAction.Rejected)
                {
                    // Update all approvals with the same RequisitionId
                    var relatedApprovals = await _context.RequisitionApprovals
                        .Where(a => a.RequisitionId == approval.RequisitionId)
                        .ToListAsync();

                    foreach (var relatedApproval in relatedApprovals)
                    {
                        relatedApproval.Action = model.NewAction;
                        relatedApproval.Comment = model.Comment;
                        relatedApproval.ActionDate = DateTime.Now;
                    }

                    await _context.SaveChangesAsync();
                }
                else if ( model.NewAction == RequisitionAction.SubmittedToApproval)
                {
                    string reciverid = await _context.Users.Where(u => u.Email == model.SentTo)
                                                  .Select(u => u.ApplicationUserId)
                                                  .FirstOrDefaultAsync();
                    // Create a new RequisitionApproval record
                    var newApproval = new RequisitionApproval
                    {
                        RequisitionId = approval.RequisitionId,
                        SentBy = applicationUserId, // Set SentBy as the previous recipient
                        SentTo = reciverid,
                        Action = RequisitionAction.SubmittedToApproval,
                        Comment = model.Comment,
                        ActionDate = DateTime.Now
                    };

                    _context.RequisitionApprovals.Add(newApproval);
                    await _context.SaveChangesAsync();
                }
                else if(model.NewAction == RequisitionAction.OnHold)
                {
                    // Update all approvals with the same RequisitionId
                    var relatedApproval = await _context.RequisitionApprovals
                        .Where(a => a.ApprovalId == model.ApprovalId)
                        .FirstOrDefaultAsync();
                    relatedApproval.Action = model.NewAction;
                    relatedApproval.Comment = model.Comment;
                    relatedApproval.ActionDate = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

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

            model.UsersEmail = usersEmail;
            // Handle invalid model state (if any)
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFile(RequisitionFileViewModel viewModel, IFormFile file)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve the current user's ApplicationUserId
            var applicationUserId = await _context.Users
                                                  .Where(u => u.Id == currentUserId)
                                                  .Select(u => u.ApplicationUserId)
                                                  .FirstOrDefaultAsync();

            // Check if the current user's ApplicationUserId is in SentBy or SentTo for the given RequisitionId
            var isAuthorized = await _context.RequisitionApprovals
                                              .AnyAsync(a => a.RequisitionId == viewModel.RequisitionId &&
                                                             (a.SentTo == applicationUserId || a.SentBy == applicationUserId));

            if (!isAuthorized)
            {
                // If the user is not authorized, redirect to the Index page or an Access Denied page
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Generate a unique file name to avoid collisions
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Create the RequisitionSupplement entity
                    var requisitionSupplement = new RequisitionSupplement
                    {
                        RequisitionId = viewModel.RequisitionId,
                        FileLink = $"/uploads/{fileName}",
                        Description = viewModel.Description,
                        Number = GetNextSupplementNumber(viewModel.RequisitionId),
                        FileAddedBy=applicationUserId
                    };

                    // Add the new RequisitionSupplement to the context
                    _context.Add(requisitionSupplement);
                    await _context.SaveChangesAsync();
                }

                // Redirect to AddFile to allow adding another file
                return RedirectToAction(nameof(AddFile), new { id = viewModel.RequisitionId });
            }

            // Return the view with validation errors if needed
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddFile(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            // Retrieve the current user's ApplicationUserId
            var applicationUserId = await _context.Users
                                                  .Where(u => u.Id == currentUserId)
                                                  .Select(u => u.ApplicationUserId)
                                                  .FirstOrDefaultAsync();

            // Check if the current user's ApplicationUserId is in SentBy or SentTo for the given RequisitionId
            var isAuthorized = await _context.RequisitionApprovals
                                              .AnyAsync(a => a.RequisitionId == id &&
                                                             (a.SentTo == applicationUserId || a.SentBy == applicationUserId));

            if (!isAuthorized)
            {
                // If the user is not authorized, redirect to the Index page or an Access Denied page
                return RedirectToAction(nameof(Index));
            }
            // Retrieve existing supplements to display them below the form
            var existingSupplements = await _context.RequisitionSupplements
                .Where(rs => rs.RequisitionId == id)
                .ToListAsync();

            var viewModel = new RequisitionFileViewModel
            {
                RequisitionId = id,
                ExistingSupplements = existingSupplements
            };

            return View(viewModel);
        }




        // Helper method to get the next number for supplements
        private int GetNextSupplementNumber(string requisitionId)
        {
            var lastSupplement = _context.RequisitionSupplements
                .Where(rs => rs.RequisitionId == requisitionId)
                .OrderByDescending(rs => rs.Number)
                .FirstOrDefault();

            return (lastSupplement?.Number ?? 0) + 1;
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
