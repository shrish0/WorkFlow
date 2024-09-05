using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WorkFlow.Data.DataAccess;
using WorkFlow.Models;
using WorkFlow.Utility;
using WorkFlow.Stores;
using WorkFlowWeb.middleware;

var builder = WebApplication.CreateBuilder(args);

// Email Configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Database Context
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserStore<ApplicationUserStore>()
    .AddDefaultTokenProviders();

// Email Sender Service
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Razor Pages and MVC
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(2); // User will be signed out after 30 minutes of inactivity
    options.SlidingExpiration = true; // Reset the expiration time if the user is active
});

// Register the CheckLockoutMiddleware
//builder.Services.AddScoped<CheckLockoutMiddleware>();

var app = builder.Build();

// Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


// Add the CheckLockoutMiddleware before Authentication and Authorization


app.UseMiddleware<CheckLockoutMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapRazorPages();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
