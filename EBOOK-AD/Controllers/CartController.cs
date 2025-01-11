using EBOOK_AD.Data;
using EBOOK_AD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add a book to the cart
    [HttpPost]
    public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            // Return a JSON response indicating the user is not logged in
            return Json(new { success = false, message = "User not logged in" });
        }

        try
        {
            // Check if the book already exists in the user's cart
            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);

            if (existingCartItem != null)
            {
                // Update quantity if book is already in cart
                existingCartItem.Quantity += quantity;
            }
            else
            {
                // Add new item to cart
                var cartItem = new Cart
                {
                    UserId = (int)userId,
                    BookId = bookId,
                    Quantity = quantity
                };
                _context.Carts.Add(cartItem);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return success response
            return Json(new { success = true, message = "Item added to cart!" });
        }
        catch (Exception ex)
        {
            // Return error response
            return Json(new { success = false, message = ex.Message });
        }
    }


    // Get the total count of items in the cart
    public async Task<IActionResult> GetCartItemCount()
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            // Return item count as 0 if user is not logged in
            return Json(new { itemCount = 0 });
        }

        // Calculate the total number of items in the user's cart
        var totalItemsInCart = await _context.Carts
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Quantity);

        // Return total item count as JSON
        return Json(new { itemCount = totalItemsInCart });
    }

    // View the items in the cart
    public async Task<IActionResult> ViewCart()
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            // Redirect to login if user is not logged in
            return RedirectToAction("Login", "Home");
        }

        // Get the user's cart items along with the associated book details
        var cartItems = await _context.Carts
            .Include(c => c.Book) // Include book details in cart items
            .Where(c => c.UserId == userId)
            .ToListAsync();

        // Return the cart items to the view
        return View(cartItems);
    }

    // Remove item from the cart
    public async Task<IActionResult> RemoveFromCart(int cartId)
    {
        // Find the cart item by its ID
        var cartItem = await _context.Carts.FindAsync(cartId);

        if (cartItem != null)
        {
            // Remove the cart item from the database
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        // Redirect back to the ViewCart action after removal
        return RedirectToAction("ViewCart");
    }





    public async Task<IActionResult> Checkout()
    {
        var userId = GetLoggedInUserId(); // Replace with your authentication logic

        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        // Fetch cart items for the logged-in user
        var cartItems = await _context.Carts
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems == null || !cartItems.Any())
        {
            return RedirectToAction("ViewCart");
        }

        // Optionally, calculate the total and pass it to the view
        var cartTotal = cartItems.Sum(c => c.Quantity * c.Book.Price);

        ViewBag.CartTotal = cartTotal;

        // Return a view for checkout (create a Checkout view if needed)
        return View(cartItems);
    }

    public async Task<IActionResult> ConfirmOrder(string shippingAddress, string paymentStatus)
    {
        var userId = GetLoggedInUserId();

        if (userId == null)
        {
            return RedirectToAction("Login", "Home");
        }

        var cartItems = await _context.Carts
            .Include(c => c.Book)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
        {
            return RedirectToAction("ViewCart");
        }

        // Calculate the total order cost
        var orderTotal = cartItems.Sum(c => c.Quantity * c.Book.Price);

        // Create an Order record
        var order = new Order
        {
            UserId = userId.Value,
            TotalAmount = orderTotal,
            OrderDate = DateTime.Now,
            Status = "Pending", // Set initial status
            ShippingAddress = shippingAddress,
            PaymentStatus = paymentStatus,
            DeliveryDate = DateTime.Now.AddDays(3) // Delivery in 3 days (customize as needed)
        };

        // Add the order to the database
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(); // Save the order first to get the order ID

        // Add the OrderItems to the database
        foreach (var cartItem in cartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                BookId = cartItem.BookId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Book.Price
            };

            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync(); // Save the order items to the database

        // Clear the cart after the order is placed
        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        // Pass a success message to the view
        TempData["OrderPlacedMessage"] = "Your order has been placed successfully!";

        // Redirect to the confirmation page or render it directly
        return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
    }


    // Private method to get the logged-in user's ID (Placeholder logic, replace as needed)
    private int? GetLoggedInUserId()
    {
        // Implement actual user authentication logic to fetch the logged-in user's ID
        // Example for using session or user claims (replace with real logic)
        // return HttpContext.User?.Identity?.Name;

        return 1; // Placeholder for demonstration, replace with real user ID retrieval logic
    }
}
