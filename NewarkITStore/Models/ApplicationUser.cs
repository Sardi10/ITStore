using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace NewarkITStore.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public string Status { get; set; } // e.g., Regular, Silver, Gold, Platinum

        public DateTime? DateOfBirth { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string? PhoneNumber { get; set; } 

        public ICollection<CreditCard>? CreditCards { get; set; }
        public ICollection<ShippingAddress>? ShippingAddresses { get; set; }
    }
}
