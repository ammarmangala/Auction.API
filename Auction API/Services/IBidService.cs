using Auction_API.Dto.Bid;

namespace Auction_API.Services;

public interface IBidService
{
    BidSummaryDTO GetById(int id);
    Task<bool> PlaceBidAsync(BidDTO dto, string userId);
}