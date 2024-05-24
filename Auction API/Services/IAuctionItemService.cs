using Auction_API.Dto.AuctionItem;
using Auction_API.Dto.Bid;
using Auction_API.Entities;

namespace Auction_API.Services
{
    public interface IAuctionItemService
    {
        Task<AuctionItem> CreateAuctionItem(CreateAuctionItemDTO dto, string userId);
        Task<bool> CancelAuctionItem(int itemId, string userId);
        Task<IEnumerable<AuctionItemDTO>> GetSoldItems(string userId);
        Task<IEnumerable<Bid>> GetBidsForItem(int itemId);
        Task<AuctionItemDTO> GetAuctionItemById(int itemId);
        Task<List<AuctionItemDTO>> SearchItemsAsync(AuctionItemSearchDTO dto);
        Task<bool> PlaceBidAsync(BidDTO dto, string userId);
        Task<List<AuctionItemDTO>> GetPurchasedItemsByUserIdAsync(string userId);
        Task<bool> SimulatePaymentAsync(int itemId, string userId);
    }
}
