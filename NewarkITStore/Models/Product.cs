public class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal RecommendedPrice { get; set; }
    public int QuantityInStock { get; set; }

    public int ProductTypeId { get; set; }
    public ProductType ProductType { get; set; }

    public ICollection<BasketItem> BasketItems { get; set; }
}
