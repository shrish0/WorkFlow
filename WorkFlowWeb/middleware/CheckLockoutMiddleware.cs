using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WorkFlow.Models;

namespace WorkFlowWeb.middleware { 

        public class CheckLockoutMiddleware
        {
            private readonly RequestDelegate _next;

            public CheckLockoutMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    var user = await userManager.GetUserAsync(context.User);
                    if (user != null && await userManager.IsLockedOutAsync(user))
                    {
                        await context.SignOutAsync();
                        context.Response.Redirect("/Identity/Account/Lockout");
                        return;
                    }
                }

                await _next(context);
            }
        }
}

