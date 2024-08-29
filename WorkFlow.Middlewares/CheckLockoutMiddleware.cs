using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WorkFlow.Models;

namespace WorkFlow.Middlewares
{
    public class CheckLockoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CheckLockoutMiddleware(RequestDelegate next, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _next = next;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(context.User);
                if (user != null && await _userManager.IsLockedOutAsync(user))
                {
                    await _signInManager.SignOutAsync();
                    context.Response.Redirect("/Identity/Account/Login?locked=true");
                    return;
                }
            }

            await _next(context);
        }
    }

}
