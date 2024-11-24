using EBOOK_AD.Data;
using EBOOK_AD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }
  

    // Add a book to the cart
    public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            return RedirectToAction("Login", "Home"); // Redirect to login if not logged in
        }

        // Check if the book already exists in the user's cart
        var existingCartItem = await _context.Carts
            .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += quantity; // Update quantity
        }
        else
        {
            var cartItem = new Cart
            {
                UserId = userId,
                BookId = bookId,
                Quantity = quantity
            };

            _context.Carts.Add(cartItem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Books"); // Redirect back to book list
    }

    // View the cart
    public async Task<IActionResult> ViewCart()
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var cartItems = await _context.Carts
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(cartItems);
    }

    // Remove item from cart
    public async Task<IActionResult> RemoveFromCart(int cartId)
    {
        var cartItem = await _context.Carts.FindAsync(cartId);

        if (cartItem != null)
        {
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("ViewCart");
    }

    private int GetLoggedInUserId()
    {
        // Replace with logic to get the logged-in user ID
        return 1; // Placeholder
    }
}
