using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment1.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Areas.Identity.Pages.Account
{
    public class AdminModel : PageModel
    {
        private readonly UserManager<User> _userManager;
          private readonly SignInManager<User> _signInManager;

        public AdminModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public async Task OnGetAsync()
        {
            var allUsers = _userManager.Users.AsEnumerable();
            var userViewModels = allUsers.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                Approved = user.Approved
            }).ToList();

            // Apply filtering
            if (!string.IsNullOrEmpty(Filter))
            {
                var filterLower = Filter.ToLower();
                userViewModels = userViewModels.Where(u =>
                    (u.FirstName?.ToLower().Contains(filterLower) ?? false) ||
                    (u.LastName?.ToLower().Contains(filterLower) ?? false) ||
                    (u.Email?.ToLower().Contains(filterLower) ?? false) ||
                    (u.Role?.ToLower().Contains(filterLower) ?? false) ||
                    u.Approved
                ).ToList();
            }

            // Calculate pagination
            TotalPages = (int)Math.Ceiling(userViewModels.Count / (double)PageSize);
            TotalPages = TotalPages == 0 ? 1 : TotalPages;

            Users = userViewModels.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
        }

        public async Task<IActionResult> OnPostApproveAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Toggle approval status
            user.Approved = !user.Approved;
            await _userManager.UpdateAsync(user);

            return RedirectToPage(new { filter = Filter, currentPage = CurrentPage });
        }
        public async Task<IActionResult> OnPostChangeRoleAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Role = user.Role == "Contributor" ? "Admin" : "Contributor";
            await _userManager.UpdateAsync(user);
           
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage(new { filter = Filter, currentPage = CurrentPage });
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToPage(new { filter = Filter, currentPage = CurrentPage });
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Boolean Approved { get; set; }
    }
}