using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;




namespace Assignment1.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
    
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
            
            if (!user.Approved)
            {
                ModelState.AddModelError("", "Your account has not been approved yet.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Article");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        public IActionResult Register()
        {
            var model = new RegisterViewModel(); 
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

           
            var user = new User { UserName = model.Input.Email, Email = model.Input.Email, FirstName = model.Input.FirstName, LastName = model.Input.LastName };
            var result = await _userManager.CreateAsync(user, model.Input.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Article");
            }

            foreach (var error in result.Errors)
            {
                if (error.Code.Contains("Password", StringComparison.OrdinalIgnoreCase) 
                    || error.Description.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Input.Password", error.Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Article");
        }
    }
}
