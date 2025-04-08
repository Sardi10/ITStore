using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewarkITStore.Models;
public class BasketItem
{
    public int BasketItemId { get; set; }
    [Required]
    public string UserId { get; set; } // Linked to logged-in user
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
}
