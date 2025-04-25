using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewarkITStore.Models
{
    public class CreditCard
    {
        public int CreditCardId { get; set; }

        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be exactly 16 digits.")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
        [Display(Name = "Security Code (CVV)")]
        public string SecurityCode { get; set; }

        [Required]
        [Display(Name = "Name on Card")]
        public string CardHolderName { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        public string BillingStreet { get; set; }

        [Required]
        public string BillingCity { get; set; }

        public string? BillingState { get; set; }

        [Required]
        public string BillingCountry { get; set; }

        [Required]
        [Display(Name = "ZIP Code")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Enter a valid ZIP code.")]
        public string BillingZip { get; set; }


        [Required]
        [Display(Name = "Card Type")]
        public string CardType { get; set; } // Visa, MasterCard, etc.

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        // Foreign Key to ApplicationUser
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
