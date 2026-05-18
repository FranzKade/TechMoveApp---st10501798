using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace TechMoveApp.Controllers
{
    public class AccountController : Controller
    {
        // 1. GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If the user is already logged in, don't show the form; send them to the main dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Contracts");
            }

            // Store where the user was trying to go before being intercepted
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // 2. POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Operational Security Check (Hardcoded Operator Matrix for this tier)
            if (username == "admin" && password == "password")
            {
                // Create claims identity detailing who the authenticated operator is
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Administrator"),
                    new Claim("SystemOperatorToken", Guid.NewGuid().ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false, // Session expires when browser closes for defense-in-depth
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                // Issue secure, encrypted authentication cookie to the browser pipeline
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // If they were trying to access a specific page (e.g., creating a service request directly), send them there.
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Contracts");
            }

            // Validation Failure Feedback Path
            ModelState.AddModelError(string.Empty, "Invalid system operator credentials or security passkey token.");
            return View();
        }

        // 3. POST/GET: /Account/Logout
        // Can be triggered via a button post-back or a direct navigation route
        public async Task<IActionResult> Logout()
        {
            // Erase the secure tracking cookie from the browser completely
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Provide explicit confirmation feedback messaging to the login panel
            TempData["LogoutMessage"] = "System session successfully terminated. Gateway locked.";

            return RedirectToAction(nameof(Login));
        }

        // 4. GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}