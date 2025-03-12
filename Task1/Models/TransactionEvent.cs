using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task1.Models
{
    public class TransactionEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long TransactionId { get; set; }

        [Required]
        public string EventType { get; set; } // e.g., Deposit, Withdrawal, Account Status Change

        [Required]
        public string Details { get; set; } // Event description

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}