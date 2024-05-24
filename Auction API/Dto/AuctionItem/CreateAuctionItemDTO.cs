namespace Auction_API.Dto.AuctionItem
{
    public class CreateAuctionItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double StartingPrice { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int CategoryId { get; set; } // The status of the item (e.g., "Initial", "Cancelled", "Paid")
        
    }
}
