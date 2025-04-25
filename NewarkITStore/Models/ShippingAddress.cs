using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewarkITStore.Models
{
    public class ShippingAddress
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Address Name")]
        public string AddressName { get; set; } // Unique per user

        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "Street Number")]
        public string StreetNumber { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Display(Name = "ZIP Code")]
        [Required]
        public string Zip { get; set; }
    }
}
