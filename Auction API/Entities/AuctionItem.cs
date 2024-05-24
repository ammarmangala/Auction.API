using Auction_API.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auction_API.Entities;

public class AuctionItem : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double StartingPrice { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public Status Status { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public string SellerId { get; set; }
    public virtual Category Category { get; set; }
    public virtual ICollection<Bid> Bids { get; set; }
}
public enum Status
{
    Initial,
    Cancelled,
    Paid
}