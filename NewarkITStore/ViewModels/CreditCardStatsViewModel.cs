namespace NewarkITStore.ViewModels
{
    public class CreditCardStatsViewModel
    {
        public string UserEmail { get; set; }
        public string MaskedCardNumber { get; set; }
        public decimal TotalCharged { get; set; }
        public List<CreditCardStatsViewModel> Data { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
