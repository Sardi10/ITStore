public class BasketItem
{
    public int BasketItemId { get; set; }

    public string UserId { get; set; } // Linked to logged-in user
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
}
