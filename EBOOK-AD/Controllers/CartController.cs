using EBOOK_AD.Data;
using EBOOK_AD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EBOOK_AD.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cart
        public IActionResult Index(int userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();
            return View(cartItems);
        }

        // GET: Cart/AddToCart/{bookId}
        public IActionResult AddToCart(int bookId, int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.BookId == bookId && c.UserId == userId);

            if (cart != null)
            {
                cart.Quantity += 1; // If book is already in the cart, increase quantity
                _context.Update(cart);
            }
            else
            {
                var newCart = new Cart
                {
                    BookId = bookId,
                    UserId = userId,
                    Quantity = 1
                };
                _context.Add(newCart);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", new { userId = userId });
        }

        // GET: Cart/RemoveFromCart/{cartId}
        public IActionResult RemoveFromCart(int cartId, int userId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId);

            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { userId = userId });
        }

        // GET: Cart/UpdateQuantity/{cartId}
        public IActionResult UpdateQuantity(int cartId, int quantity, int userId)
        {
            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId);

            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                _context.Update(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { userId = userId });
        }

        // POST: Cart/Checkout
        [HttpPost]
        public IActionResult Checkout(int userId)
        {
            var cartItems = _context.Carts.Where(c => c.UserId == userId).ToList();

            // Implement checkout logic here: Update orders, reduce stock, etc.

            // After checkout, clear the cart
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect to homepage after checkout
        }
    }
}
