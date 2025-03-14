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
        public DbSet<LogEntry> LogEntries { get; set; } // Add LogEntry
        public DbSet<TransactionEvent> TransactionEvents { get; set; }
        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionLog>()
                .Property(t => t.Id)
                .UseIdentityAlwaysColumn(); // PostgreSQL bigserial

            modelBuilder.Entity<TransactionLog>()
                .HasIndex(t => t.AccountId);

            modelBuilder.Entity<LogEntry>()
                .Property(l => l.Id)
                .UseIdentityAlwaysColumn(); // PostgreSQL bigserial

            modelBuilder.Entity<LogEntry>()
                .HasIndex(l => l.RequestId)
                .HasName("IDX_RequestId");

            modelBuilder.Entity<LogEntry>()
                .HasIndex(l => l.Timestamp)
                .HasName("IDX_Timestamp");
        }
    }
}