using NewarkITStore.Models;

namespace NewarkITStore.ViewModels;
public class CheckoutViewModel
{
    public List<BasketItem> BasketItems { get; set; }
    public List<ShippingAddress> ShippingAddresses { get; set; }
    public List<CreditCard> SavedCards { get; set; }

    // Optional selections:
    public int? SelectedShippingAddressId { get; set; }
    public int? SelectedCreditCardId { get; set; }
}
