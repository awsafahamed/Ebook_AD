using EBOOK_AD.Data;
using EBOOK_AD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Index action to display all orders
    public IActionResult Index()
    {
        var orders = _context.Orders.ToList();
        return View(orders);
    }

    // Edit action to update order status
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var order = _context.Orders.Find(id);
        if (order == null)
            return NotFound();

        var model = new EditOrderViewModel
        {
            OrderId = order.OrderId,
            Status = order.Status
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(EditOrderViewModel model)
    {
        if (ModelState.IsValid)
        {
            var order = _context.Orders.Find(model.OrderId);
            if (order != null)
            {
                order.Status = model.Status;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return View(model);
    }

    // Details action to view details of an order
    public IActionResult Details(int id)
    {
        var order = _context.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.OrderId == id);
        if (order == null)
            return NotFound();

        return View(order);
    }
}
