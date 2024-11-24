using EBOOK_AD.Data;
using EBOOK_AD.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace eBookWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.Include(o => o.OrderItems).ToListAsync();
            return View(orders);
        }

        // GET: Checkout
        public IActionResult Checkout()
        {
            // Example: Get current user's cart (this should ideally be stored in a session or a database)
            // Here, we retrieve the cart for the currently logged-in user.
            var userId = 1; // For example, replace this with actual logged-in user ID using: User.Identity.Name
            var cartItems = _context.OrderItems
                .Include(oi => oi.Order) // Ensure that we include the order
                .Where(oi => oi.Order.UserId == userId && oi.Order.Status == "Pending")
                .ToList();

            var totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            var model = new CheckoutViewModel
            {
                OrderItems = cartItems,
                TotalAmount = totalAmount,
                UserId = userId // Ideally, replace with actual logged-in user ID
            };

            return View(model);
        }

        // POST: Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Order
                var order = new Order
                {
                    UserId = model.UserId,
                    TotalAmount = model.TotalAmount,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    ShippingAddress = model.ShippingAddress
                };

                _context.Orders.Add(order);
                _context.SaveChanges(); // Save the order first to get the OrderId

                // Add OrderItems to the new order
                foreach (var item in model.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                _context.SaveChanges(); // Save the order items

                // Redirect to a confirmation page or the order summary page
                return RedirectToAction("OrderConfirmation", new { id = order.OrderId });
            }

            // If model is invalid, redisplay the checkout page
            return View(model);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId, UserId, TotalAmount, OrderDate, Status, ShippingAddress")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order != null)
            {
                _context.OrderItems.RemoveRange(order.OrderItems); // Remove associated items
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
