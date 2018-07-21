using DemoMobileApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoMobileApi.DAL
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
