using System;
using System.ComponentModel.DataAnnotations;

namespace Task1.Models
{
    public class Transaction
    {
        [Key]
        public long Id { get; set; }

        public long FromAccountId { get; set; }
        public long ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }

        // Store translations for transaction descriptions
        public string DescriptionEn { get; set; }
        public string DescriptionEs { get; set; }
        public string DescriptionFr { get; set; }

        public string GetLocalizedDescription(string language) =>
            language switch
            {
                "es" => DescriptionEs,
                "fr" => DescriptionFr,
                _ => DescriptionEn
            };
    }
}