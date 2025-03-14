using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task1.Models
{
    public class Account
    {
        [Key]
        public long Id { get; set; }

        public decimal Balance { get; set; }
        
        //Added new attributes for translation 

        // Store translations for account names
        public string NameEn { get; set; }
        public string NameEs { get; set; }
        public string NameFr { get; set; }

        // Method to get the localized name
        public string GetLocalizedName(string language) =>
            language switch
            {
                "es" => NameEs,
                "fr" => NameFr,
                _ => NameEn
            };
    }
}