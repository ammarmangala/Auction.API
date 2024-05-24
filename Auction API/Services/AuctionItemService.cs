using Auction_API.Data;
using Auction_API.Dto.AuctionItem;
using Auction_API.Dto.Bid;
using Auction_API.Entities;
using Auction_API.GlobalExceptionHandling;
using Auction_API.Repositories;

namespace Auction_API.Services
{
    public class AuctionItemService : IAuctionItemService
    {
        private readonly IAuctionItemRepository _auctionItemRepository;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public AuctionItemService(IAuctionItemRepository auctionItemRepository, IUserService userService, ApplicationDbContext context)
        {
            _auctionItemRepository = auctionItemRepository;
            _userService = userService;
            _context = context;
        }

        public async Task<AuctionItem> CreateAuctionItem(CreateAuctionItemDTO dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new CustomException("The auction item name must not be blank.");
            }

            if (dto.StartingPrice < 0)
            {
                throw new CustomException("Starting price cannot be negative.");
            }

            var subscriptionType = await _userService.GetSubscriptionTypeAsync(userId);
            DateTime endDateTime;
            if (subscriptionType == "Gold" || subscriptionType == "Platinum")
            {
                if (dto.EndDateTime <= dto.StartDateTime.AddHours(12))
                {
                    throw new CustomException("The end time must be at least 12 hours after the start time for Gold or Platinum users.");
                }
                endDateTime = dto.EndDateTime;
            }
            else
            {
                // Voor Free gebruikers wordt de einddatum automatisch op 3 dagen na de startdatum ingesteld
                endDateTime = dto.StartDateTime.AddDays(3);
            }

            // Create a new auction item
            var auctionItem = new AuctionItem
            {
                Name = dto.Name,
                Description = dto.Description,
                StartingPrice = dto.StartingPrice,
                StartDateTime = dto.StartDateTime,
                EndDateTime = endDateTime,
                SellerId = userId,
                Status = Status.Initial,
                CategoryId = dto.CategoryId
            };

            return await _auctionItemRepository.AddAsync(auctionItem);
        }

        public async Task<bool> CancelAuctionItem(int itemId, string userId)
        {
            var auctionItem = await _context.AuctionItems.FindAsync(itemId);
            if (auctionItem == null)
            {
                throw new CustomException("Auction item not found.");
            }

            if (auctionItem.SellerId != userId)
            {
                throw new CustomException("Only the seller can cancel the sale.");
            }

            auctionItem.Status = Status.Cancelled; // Use the enumeration value here
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<AuctionItemDTO>> GetSoldItems(string userId)
        {
            return await _auctionItemRepository.GetSoldItemsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Bid>> GetBidsForItem(int itemId)
        {
            var auctionItem = await _auctionItemRepository.GetByIdAsync(itemId);
            if (auctionItem == null)
            {
                throw new CustomException("The auction item was not found and therefore has no bids.");
            }

            return await _auctionItemRepository.GetBidsByItemIdAsync(itemId);
        }

        public async Task<AuctionItemDTO> GetAuctionItemById(int itemId)
        {
            var auctionItem = await _auctionItemRepository.GetByIdAsync(itemId);
            if (auctionItem == null)
            {
                throw new CustomException("Auction item not found");
            }
            return auctionItem;
        }

        public async Task<List<AuctionItemDTO>> SearchItemsAsync(AuctionItemSearchDTO dto)
        {
            var items = await _auctionItemRepository.SearchItemsAsync(dto.Categories, dto.MaxPrice);

            // Sort the items based on the bidding end time
            var sortedItems = items.OrderBy(item => item.EndDateTime).ToList();

            return sortedItems;
        }

        public async Task<bool> PlaceBidAsync(BidDTO dto, string userId)
        {
            var auctionItem = await _auctionItemRepository.GetByIdAsync(dto.ItemId);
            if (auctionItem == null || auctionItem.Status == Status.Cancelled || auctionItem.EndDateTime <= DateTime.Now || auctionItem.SellerId == userId)
            {
                return false;
            }

            // Check if the bid is at least 5% higher than the current highest bid
            var highestBid = await _auctionItemRepository.GetHighestBidForItemAsync(dto.ItemId);
            if (highestBid != null && dto.Amount <= highestBid.Amount * 1.05)
            {
                return false;
            }

            // Round the bid amount to the nearest €0.50
            dto.Amount = Math.Round(dto.Amount * 2) / 2;

            var bid = new Bid
            {
                AuctionItemId = dto.ItemId,
                Amount = dto.Amount,
                BidderId = userId,
                BidDateTime = DateTime.UtcNow
            };

            try
            {
                await _auctionItemRepository.PlaceBidAsync(bid);
            }
            catch (Exception)
            {
                // Log the exception and return false
                return false;
            }

            return true;
        }

        public async Task<List<AuctionItemDTO>> GetPurchasedItemsByUserIdAsync(string userId)
        {
            var items = await _auctionItemRepository.GetPurchasedItemsByUserIdAsync(userId);
            return items;
        }

        public async Task<bool> SimulatePaymentAsync(int itemId, string userId)
        {
            var auctionItem = await _auctionItemRepository.GetByIdAsync(itemId);
            if (auctionItem == null)
            {
                throw new CustomException("Auction item not found.");
            }

            if (auctionItem.SellerId == userId)
            {
                throw new CustomException("You cannot pay for your own item.");
            }
            
            //auctionItem.Status = Status.Paid;
            
            await _auctionItemRepository.UpdateAuctionItem(auctionItem.Id);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
