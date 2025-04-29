using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using NewarkITStore.Models;

namespace NewarkITStore.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }

        [Required(ErrorMessage = "Please select a product")]
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }

        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "Offer price must be positive")]
        public decimal OfferPrice { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
