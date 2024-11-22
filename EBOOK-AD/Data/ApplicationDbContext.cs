
using Microsoft.EntityFrameworkCore;
using EBOOK_AD.Models;

namespace EBOOK_AD.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Report> Reports { get; set; }

    }
}
