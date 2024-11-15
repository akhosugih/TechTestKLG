using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using TechTestKLG.Models;
using Microsoft.Extensions.Logging;

namespace TechTestKLG.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(UserManager<Users> userManager, SignInManager<Users> signInManager, ILogger<LoginModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public class LoginViewModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByNameAsync(Input.Username);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToPage("/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return Page();
            }
        }
    }
}
