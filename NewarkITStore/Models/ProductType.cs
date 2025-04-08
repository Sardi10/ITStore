using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace NewarkITStore.Models;
public class ProductType
{
    public int ProductTypeId { get; set; }
    [Required]
    public string Name { get; set; } // e.g., Laptop, Desktop, Printer, Accessory

    public string? CPU { get; set; }
    public float? Weight { get; set; }
    public float? BatteryLife { get; set; }
    public string? Resolution { get; set; }
    public string? PrinterType { get; set; }

    [BindNever]
    public ICollection<Product> Products { get; set; } = new List<Product>();

}
