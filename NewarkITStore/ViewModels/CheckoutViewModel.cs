using NewarkITStore.Models;

namespace NewarkITStore.ViewModels;
public class CheckoutViewModel
{
    public List<BasketItem> BasketItems { get; set; }
    public List<ShippingAddress> ShippingAddresses { get; set; }
}
