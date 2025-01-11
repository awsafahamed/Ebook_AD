using EBOOK_AD.Data;
using EBOOK_AD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace EBOOK_AD.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            // Count the total number of books, users, and orders
            ViewData["TotalBooks"] = await _context.Books.CountAsync();
            ViewData["TotalUsers"] = await _context.Users.CountAsync(); // Assuming the Users table exists
            ViewData["TotalOrders"] = await _context.Orders.CountAsync(); // Assuming the Orders table exists

            return View(await _context.Books.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Category,Author,Price,Stock,Description")] Book book, IFormFile ImageFile)
        {
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                if (ImageFile != null)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", ImageFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                    book.ImageUrl = "/images/" + ImageFile.FileName; // Save the relative path
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Category,Author,Price,Stock,Description,ImageUrl")] Book book, IFormFile ImageFile)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Generate a unique file name for the new image
                        var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(ImageFile.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

                        // Save the new image
                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // Set the new image URL
                        book.ImageUrl = "/images/" + uniqueFileName;

                        // Delete the old image if it exists
                        if (!string.IsNullOrEmpty(book.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath); // Delete the old image from the server
                            }
                        }
                    }
                    else
                    {
                        // If no new image is uploaded, retain the old one
                        var existingBook = await _context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.BookId == book.BookId);
                        book.ImageUrl = existingBook?.ImageUrl;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book.ImageUrl != null)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath); // Delete the image from the server
                }
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // API: GetBooks
        [HttpGet]
        public JsonResult GetBooks()
        {
            var books = _context.Books.ToList(); // Fetch all books from the database
            return Json(books);
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
