using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Data.DataAccess;
using WorkFlow.Models;
using WorkFlow.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WorkFlowWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ManageUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ManageUserController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public ManageUserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<ManageUserController> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.ApplicationUsers.ToListAsync();
            var currentUserId = _userManager.GetUserId(User); // Get current logged-in user's ID

            var model = new UserListViewModel
            {
                Users = users.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    ApplicationUserId=u.ApplicationUserId,
                    CreatedBy=u.CreatedBy,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,
                    PhoneNumber = u.PhoneNumber,
                    LockoutEnd = u.LockoutEnd,
                    BlockedBy=u.BlockedBy
                }).ToList(),
                CurrentUserId = currentUserId
            };

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserCreateViewModel
            {
                RoleList = roles.Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var creator = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Created = DateTime.Now,
                    Modified = DateTime.Now,
                    CreatedBy=creator.ApplicationUserId,
                    ClearanceLevel=model.ClearanceLevel
                };
               
                var result = await _userManager.CreateAsync(user, "M(Zr[7J\\<?5$UYh{g:Bzxw'");
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                        if (!roleResult.Succeeded)
                        {
                            AddErrors(roleResult);
                        }
                    }

                    String emailContent = GenerateEmailContent(user.FirstName, user.Email);
                    await _emailSender.SendEmailAsync(model.Email,
                       "Confirm your email",
                        emailContent);

                    return RedirectToAction(nameof(Index));
                }
                AddErrors(result);
            }

            var roles = await _roleManager.Roles.ToListAsync();
            model.RoleList = roles.Select(role => new SelectListItem
            {
                Value = role.Name,
                Text = role.Name
            }).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users.FindAsync(id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Address,Email,PhoneNumber,ClearanceLevel,ApplicationUserId")] ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _context.Users.FindAsync(id);
                    if (userToUpdate == null)
                    {
                        return NotFound();
                    }

                    // Update the fields manually
                    userToUpdate.FirstName = applicationUser.FirstName;
                    userToUpdate.LastName = applicationUser.LastName;
                    userToUpdate.Address = applicationUser.Address;
                    userToUpdate.Email = applicationUser.Email;
                    userToUpdate.PhoneNumber = applicationUser.PhoneNumber;
                    userToUpdate.ClearanceLevel = applicationUser.ClearanceLevel;
                    userToUpdate.Modified = DateTime.Now;

                    _context.Update(userToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(applicationUser);
        }


        public async Task<IActionResult> Block (string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var applicationUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

       

        public string GenerateEmailContent(string userName, string email)
         {
                return $@"
                <p>Hi {userName},</p>

                <p>Congratulations! your account on the Maharshi Ayurveda  has been created successfully.</p>

                <p><b>To activate your account do the following step:</b></p>

                <p>Please find below your login credentials. You can save it for logging into the portal for tracking your application status and other updates to your profile.</p>

                <p><b>Portal URL:</b> <a href='https://localhost:7092/Identity/Account/Login'>click here to login</a></p>
                <p><b>User Name:</b> {email}</p>
                <p><b>Password:</b> Please login using your password. If you are logging in first time, please follow the below steps to reset your password:</p>
                <ol>
                    <li>Open portal login page</li>
                    <li>Activate your account by clicking on Resend Email confirmation</li>
                    <li>Go to mail to activate your account</li>
                    <li>After that Click on Forget Password</li>
                    <li>To set a new Password</li>
                </ol>

                <p>Best regards,</p>
                <p>Maharshi Ayurveda Team</p>
                ";
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LockUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Get the ID of the current admin performing the lock action
            var adminId = _userManager.GetUserId(User);
            var applicationUserID = await _context.Users
                                                     .Where(u => u.Id == adminId)
                                                     .Select(u => u.ApplicationUserId)
                                                     .FirstOrDefaultAsync();

            // Lock the user account for a maximum time span
            var lockoutEndDate = DateTimeOffset.UtcNow.AddYears(100);
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
            if (result.Succeeded)
            {
                // Set the BlockedBy field
                user.BlockedBy = applicationUserID;

                // Save the changes to the database
                _context.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} has been locked out by {adminId}.");
                return RedirectToAction(nameof(Index));
            }
            AddErrors(result);
            return View("Index");
        }

        // Method to unlock a user account
        [HttpPost]
        public async Task<IActionResult> UnlockUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Unlock the user by setting LockoutEnd to a past date (now)
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            if (result.Succeeded)
            {
                // Clear the BlockedBy field
                user.BlockedBy = null;

                // Save the changes to the database
                _context.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {user.Email} has been unlocked.");
                return RedirectToAction(nameof(Index));
            }
            AddErrors(result);
            return View("Index");
        }


    }
}
