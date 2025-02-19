using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;


        public ArticleController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                ViewBag.Approved = user?.Approved ?? false;
            }
            var articles = await _context.Articles
                .Include(a => a.Contributor)
                .ToListAsync();
            return View(articles);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); 
            }

            var article = new Article
            {
                Title = model.Title,
                Body = model.Body,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CreateDate = DateTime.UtcNow,
                ContributorUsername = user.UserName,
                Contributor = user
            };

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return RedirectToAction("Display", "Article", new { id = article.ArticleId });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Display(int id)
        {
            var article = await _context.Articles
            .Include(a => a.Contributor)
            .FirstOrDefaultAsync(a => a.ArticleId == id);

            if (article == null)
                {
                    return NotFound();
                }

            return View(article);
        }

        [HttpGet]
        public async Task<IActionResult> UserRecords()
        {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userArticles = await _context.Articles
        .Where(a => a.ContributorUsername == user.UserName) 
        .ToListAsync();

        return View(userArticles);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == id);

        if (article == null)
        {
        return NotFound();
        }

        // Ensure only the author can edit
        var user = await _userManager.GetUserAsync(User);
        if (user == null || article.ContributorUsername != user.UserName)
        {
        return Forbid(); 
        }

        return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article model)
        {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == model.ArticleId);
        if (article == null)
        {
            return NotFound();
        }

        // Ensure only the original author can update
        var user = await _userManager.GetUserAsync(User);
        if (user == null || article.ContributorUsername != user.UserName)
        {
            return Forbid();
        }

   
        article.Title = model.Title;
        article.Body = model.Body;
        article.StartDate = model.StartDate;
        article.EndDate = model.EndDate;

        await _context.SaveChangesAsync();

        return RedirectToAction("UserRecords"); 
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
        var article = await _context.Articles.FirstOrDefaultAsync(a => a.ArticleId == id);

        if (article == null)
        {
            return NotFound();
        }

        // Ensure only the author can delete
        var user = await _userManager.GetUserAsync(User);
        if (user == null || article.ContributorUsername != user.UserName)
        {
            return Forbid(); 
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return RedirectToAction("UserRecords");
}






    }
}