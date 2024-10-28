using Microsoft.EntityFrameworkCore;
using yaya_webhook.Model;

namespace yaya_webhook.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Webhook> Webhooks { get; set; } = null!; // Use null-forgiving operator

    }

}