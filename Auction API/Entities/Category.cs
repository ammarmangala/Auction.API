using Auction_API.Entities.Base;

namespace Auction_API.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public ICollection<AuctionItem> AuctionItems { get; set; }
}