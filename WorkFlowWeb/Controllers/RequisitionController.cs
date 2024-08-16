using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Models;
using WorkFlow.ViewModels;
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

        // GET: Requisition/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new RequisitionViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                SubCategories = await _context.SubCategories.ToListAsync(),
                RequisitionHeader = new RequisitionHeader(),
                RequisitionBody = new RequisitionBody()
            };

            return View(viewModel);
        }

        // POST: Requisition/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequisitionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.RequisitionHeader.RequisitionId = Guid.NewGuid().ToString();
                viewModel.RequisitionBody.RequisitionId = viewModel.RequisitionHeader.RequisitionId;

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
    }
}
