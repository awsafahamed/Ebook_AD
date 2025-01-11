using EBOOK_AD.Models;
using EBOOK_AD.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EBOOK_AD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Inject ApplicationDbContext

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Initialize the DbContext
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // Fetch the latest books
            var latestBooks = await _context.Books
                .OrderByDescending(b => b.BookId) // Fetch the latest books by ID
                .Take(6) // Limit to 6 most recent books
                .ToListAsync();

            return View(latestBooks); // Pass the books to the view
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // Keep the user logged in
                    };

                    await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity), authProperties);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin"); // Redirect to Admin Dashboard
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // Redirect to User Home
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }


        // GET: Home/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Home/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password, // Store hashed password in production
                    Role = "User" // Default role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        // GET: Home/Books
        public async Task<IActionResult> Books()
        {
            // Fetch all books for the Books view
            var allBooks = await _context.Books.ToListAsync();
            return View(allBooks);
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int bookId, string comment, int rating)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // Retrieve UserId from session

            if (userId == null)
            {
                return Json(new { success = false, message = "Please log in to submit feedback." });
            }

            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "Invalid rating. Please provide a rating between 1 and 5." });
            }

            // Create feedback object
            var feedback = new Feedback
            {
                UserId = (int)userId,
                BookId = bookId,
                Content = comment,
                Rating = rating,
            
            };

            // Save feedback to the database
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thank you for your feedback!" });
        }


        // Handle errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
