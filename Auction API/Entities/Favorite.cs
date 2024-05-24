using Auction_API.Entities.Base;

namespace Auction_API.Entities;

public class Favorite : BaseEntity
{
    public int AuctionItemId { get; set; }
    public string UserId { get; set; }
    public AuctionItem AuctionItem { get; set; }
}