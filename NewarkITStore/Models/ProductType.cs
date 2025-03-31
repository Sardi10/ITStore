public class ProductType
{
    public int ProductTypeId { get; set; }
    public string Name { get; set; } // e.g., Laptop, Desktop, Printer, Accessory

    public string? CPU { get; set; }
    public float? Weight { get; set; }
    public float? BatteryLife { get; set; }
    public string? Resolution { get; set; }
    public string? PrinterType { get; set; }

    public ICollection<Product> Products { get; set; }
}
