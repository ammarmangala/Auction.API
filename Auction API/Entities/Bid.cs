using Auction_API.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auction_API.Entities;

public class Bid : BaseEntity
{
    public double Amount { get; set; }
    public DateTime BidDateTime { get; set; }
    public string BidderId { get; set; }

    [ForeignKey("AuctionItem")]
    public int AuctionItemId { get; set; }
    public virtual AuctionItem AuctionItem { get; set; }
}