using Auction_API.Dto.AuctionItem;
using Auction_API.Entities;

namespace Auction_API.Repositories
{
    public interface IAuctionItemRepository
    {
        Task<AuctionItem> AddAsync(AuctionItem auctionItem);
        Task<bool> CancelAsync(int itemId, string userId);
        Task<IEnumerable<AuctionItemDTO>> GetSoldItemsByUserIdAsync(string userId);
        Task<IEnumerable<Bid>> GetBidsByItemIdAsync(int itemId);
        Task<AuctionItemDTO> GetByIdAsync(int itemId);
        Task<List<AuctionItemDTO>> SearchItemsAsync(List<int> categories, double maxPrice);
        Task<Bid> GetHighestBidForItemAsync(int itemId);
        Task PlaceBidAsync(Bid bid);
        Task<List<AuctionItemDTO>> GetPurchasedItemsByUserIdAsync(string userId);
        Task UpdateAsync(AuctionItem item);
        Task UpdateAuctionItem(int itemID);
    }
}
