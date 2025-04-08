using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewarkITStore.Models;
public class Product
{
    public int ProductId { get; set; }

    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Range(0.01, 999999.99)]
    public decimal RecommendedPrice { get; set; }
    [Range(0, 100000)]
    public int QuantityInStock { get; set; }
    [Display(Name = "Image File Name")]
    public string? ImageFileName { get; set; }

    [Display(Name = "Product Type")]
    public int ProductTypeId { get; set; }
    [ValidateNever]
    public ProductType ProductType { get; set; }
    [BindNever]
    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

}
