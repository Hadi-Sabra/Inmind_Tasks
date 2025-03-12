using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task1.Models
{
    public class TransactionLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        public long AccountId { get; set; }
        
        [Required]
        public string TransactionType { get; set; } = string.Empty; // Deposit, Withdrawal
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [Required]
        public string Status { get; set; } = string.Empty; // Pending, Completed, Failed
        
        public string? Details { get; set; }
    }
}