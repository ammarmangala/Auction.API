using Auction_API.Dto.Bid;
using Auction_API.Dto.Category;
using Auction_API.Entities;

namespace Auction_API.Dto.AuctionItem
{
    public class AuctionItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double StartingPrice { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Status Status { get; set; }
        public int CategoryId { get; set; }
        public string SellerId { get; set; }
        public virtual CategoryDTO Category { get; set; }
        public ICollection<BidSummaryDTO> Bids { get; set; }
    }
}
