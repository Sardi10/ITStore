using NewarkITStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NewarkITStore.ViewModels
{
    public class OfferViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [BindNever] // Tell ASP.NET Core NOT to validate or bind this field
        public IEnumerable<SelectListItem> ProductsList { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Offer price must be greater than zero.")]
        public decimal OfferPrice { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
