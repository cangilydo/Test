using Microsoft.EntityFrameworkCore;

using Shared.Domains;

namespace Shared.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Audit> Audit { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<V_OrderDetail> V_OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<V_OrderDetail>()
                .HasNoKey();
        }
    }
}
