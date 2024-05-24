namespace Auction_API.Dto.Bid
{
    public class BidSummaryDTO
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime BidDateTime { get; set; }
        public string BidderId { get; set; }
        public int AuctionItemId { get; set; }
    }
}
