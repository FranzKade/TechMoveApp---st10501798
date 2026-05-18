using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TechMoveApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Cloud Database Access Link Configuration 
var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? "Server=tcp:eventeasedb-server.database.windows.net,1433;Initial Catalog=EventEaseDB;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication='Active Directory Default'";

builder.Services.AddDbContext<TechMoveDbContext>(options => options.UseSqlServer(connString));
builder.Services.AddControllersWithViews();

// Register network clients and pricing services
builder.Services.AddHttpClient();
builder.Services.AddScoped<TechMoveApp.Services.ExchangeRateService>();
// Add Identity Cookie configurations
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Redirect target if unauthenticated
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect target if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);  // Clear session after 20 mins of idling
    });
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();