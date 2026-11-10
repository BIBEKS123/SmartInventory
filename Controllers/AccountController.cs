

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartInventory.Models.ViewModels;

namespace SmartInventory.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        // LOGIN - GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Content("LOGIN MODELSTATE ERROR: " +
                               string.Join(" | ", errors));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    "", "Invalid email or password");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(
                "", "Invalid email or password");

            return View(model);
        }

        // REGISTER - GET
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER - POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
              
            {
               
                    return View(model);
                
            }

           

            string role;

            if (model.RegistrationType == "Admin")
            {
                string? invitationCode =
                    _configuration["AdminRegistration:InvitationCode"];

                if (string.IsNullOrWhiteSpace(invitationCode) ||
               !string.Equals(
                 model.AdminInvitationCode?.Trim(),
                    invitationCode.Trim(),
                  StringComparison.Ordinal))
                {
                    ModelState.AddModelError(
                        "AdminInvitationCode",
                        "Invalid admin invitation code");

                    return View(model);
                }

                role = "Admin";
            }
            else if (model.RegistrationType == "User")
            {
                role = "User";
            }
            else
            {
                ModelState.AddModelError(
                    "RegistrationType",
                    "Invalid account type");

                return View(model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(
                user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                await _userManager.DeleteAsync(user);
                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful! Please login.";

            return RedirectToAction("Login", "Account");
        }


        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login","Account");
        }
    }
}

