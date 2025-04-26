using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NewarkITStore.ViewModels
{
    public class UserProfileViewModel
    {
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? Status { get; set; }

        public string? ProfilePictureUrl { get; set; }
        public IFormFile? ProfilePictureFile { get; set; }

        public string? MaskedCreditCard { get; set; }
        public string? DefaultShippingAddressName { get; set; }
    }
}
