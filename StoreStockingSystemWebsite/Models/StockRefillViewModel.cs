namespace PCDSWebsite.Models
{
    public class StockRefillWebModel
    {
        public int StockId { get; set; }
        public string StoreName { get; set; }
        public int StockMissing { get; set; }
        public int RefillPriority { get; set; }
        public bool SelectedForRefilling { get; set; }
    }
}