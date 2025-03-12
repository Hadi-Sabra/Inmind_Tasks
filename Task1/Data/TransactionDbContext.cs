using Microsoft.EntityFrameworkCore;
using Task1.Models;


namespace Task1.Data
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<TransactionLog> TransactionLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionLog>()
                .Property(t => t.Id)
                .UseIdentityAlwaysColumn(); // PostgreSQL bigserial
                
            modelBuilder.Entity<TransactionLog>()
                .HasIndex(t => t.AccountId);
        }
    }
}