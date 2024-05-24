using Auction_API.Data;
using Auction_API.Dto.AuctionItem;
using Auction_API.Dto.Bid;
using Auction_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auction_API.Repositories
{
    public class AuctionItemRepository : IAuctionItemRepository
    {
        private readonly ApplicationDbContext _context;

        public AuctionItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuctionItem> AddAsync(AuctionItem auctionItem)
        {
            auctionItem = _context.AuctionItems.Add(auctionItem).Entity;
            await _context.Entry(auctionItem)
               .Reference(ai => ai.Category) 
               .LoadAsync();
            await _context.Entry(auctionItem)
               .Collection(ai => ai.Bids)
               .LoadAsync();
            await _context.SaveChangesAsync();
            return auctionItem;
        }

        public async Task<bool> CancelAsync(int itemId, string userId)
        {
            var item = await _context.AuctionItems.FindAsync(itemId);
            if (item == null || item.SellerId != userId || item.EndDateTime < DateTime.Now)
            {
                return false;
            }

            item.Status = Status.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<AuctionItemDTO>> GetSoldItemsByUserIdAsync(string userId)
        {
            return await _context.AuctionItems
                .Where(ai => ai.SellerId == userId && ai.Status == Status.Paid)
                .Select(i => new AuctionItemDTO()
                {
                    Id = i.Id,
                    Category = new Dto.Category.CategoryDTO()
                    {
                        Id = i.CategoryId,
                        Name = i.Category.Name
                    },
                    CategoryId = i.CategoryId,
                    Description = i.Description,
                    EndDateTime = i.EndDateTime,
                    Name = i.Name,
                    SellerId = i.SellerId,
                    StartDateTime = i.StartDateTime,
                    StartingPrice = i.StartingPrice,
                    Status = i.Status,
                    Bids = i.Bids.Select(i => new BidSummaryDTO()
                    {
                        Amount = i.Amount,
                        AuctionItemId = i.Id,
                        Id = i.Id,
                        BidDateTime = i.BidDateTime,
                        BidderId = i.BidderId
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Bid>> GetBidsByItemIdAsync(int itemId)
        {
             var data =  await _context.Bids.Include(ai => ai.AuctionItem)
                .Where(b => b.AuctionItemId == itemId)
                .ToListAsync();

            return data;
        }

        public async Task<AuctionItemDTO> GetByIdAsync(int itemId)
        {
           
            return await _context.AuctionItems
                .Include(ai => ai.Category)
                .Include(ai => ai.Bids).Select(i => new AuctionItemDTO()
                {
                    Id = i.Id,
                    Category = new Dto.Category.CategoryDTO()
                    {
                        Id = i.CategoryId,
                        Name = i.Category.Name
                    },
                    CategoryId = i.CategoryId,
                    Description = i.Description,
                    EndDateTime = i.EndDateTime,
                    Name = i.Name,
                    SellerId = i.SellerId,
                    StartDateTime = i.StartDateTime,
                    StartingPrice = i.StartingPrice,
                    Status = i.Status,
                    Bids = i.Bids.Select(i => new BidSummaryDTO()
                    {
                        Amount = i.Amount,
                        AuctionItemId = i.Id,
                        Id = i.Id,
                        BidDateTime = i.BidDateTime,
                        BidderId = i.BidderId
                    }).ToList()
                })
                .FirstOrDefaultAsync(ai => ai.Id == itemId);
        }

        public async Task<List<AuctionItemDTO>> SearchItemsAsync(List<int> categories, double maxPrice)
        {
            // Fetch all auction items that match the provided categories and have a starting price less than or equal to the max price
            var items = await _context.AuctionItems
                .Where(ai => categories.Contains(ai.CategoryId) && ai.StartingPrice <= maxPrice && ai.Status != Status.Cancelled && ai.EndDateTime > DateTime.Now)
                .Select(i => new AuctionItemDTO()
                {
                    Id = i.Id,
                    Category = new Dto.Category.CategoryDTO()
                    {
                        Id = i.CategoryId,
                        Name = i.Category.Name
                    },
                    CategoryId = i.CategoryId,
                    Description = i.Description,
                    EndDateTime = i.EndDateTime,
                    Name = i.Name,
                    SellerId = i.SellerId,
                    StartDateTime = i.StartDateTime,
                    StartingPrice = i.StartingPrice,
                    Status = i.Status,
                    Bids = i.Bids.Select(i => new BidSummaryDTO()
                    {
                        Amount = i.Amount,
                        AuctionItemId = i.Id,
                        Id = i.Id,
                        BidDateTime = i.BidDateTime,
                        BidderId = i.BidderId
                    }).ToList()
                })
                .ToListAsync();

            return items;
        }

        public async Task<Bid> GetHighestBidForItemAsync(int itemId)
        {
            // Fetch the highest bid for a specific auction item
            var highestBid = await _context.Bids
                .Where(b => b.AuctionItemId == itemId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();

            return highestBid;
        }

        public async Task PlaceBidAsync(Bid bid)
        {
            // Check if the bid object is null
            if (bid == null)
            {
                throw new ArgumentNullException(nameof(bid));
            }

            // Check if the AuctionItem related to the bid exists
            var auctionItem = await _context.AuctionItems.FindAsync(bid.AuctionItemId);
            if (auctionItem == null)
            {
                throw new ArgumentException("Auction item not found.");
            }

            // Add the new bid to the database
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuctionItemDTO>> GetPurchasedItemsByUserIdAsync(string userId)
        {
            // Fetch all auction items where the highest bid was placed by the provided user and the auction has ended
            var items = await _context.AuctionItems.Include(i=>i.Category).Include(i=>i.Bids)
                .Where(ai => ai.Bids.OrderByDescending(b => b.Amount).FirstOrDefault().BidderId == userId && ai.EndDateTime < DateTime.Now && ai.Status != Status.Cancelled)
                .Select(i => new AuctionItemDTO()
                {
                    Id = i.Id,
                    Category = new Dto.Category.CategoryDTO()
                    {
                        Id = i.CategoryId,
                        Name = i.Category.Name
                    },
                    CategoryId = i.CategoryId,
                    Description = i.Description,
                    EndDateTime = i.EndDateTime,
                    Name = i.Name,
                    SellerId = i.SellerId,
                    StartDateTime = i.StartDateTime,
                    StartingPrice = i.StartingPrice,
                    Status = i.Status,
                    Bids = i.Bids.Select(i => new BidSummaryDTO()
                    {
                        Amount = i.Amount,
                        AuctionItemId = i.Id,
                        Id = i.Id,
                        BidDateTime = i.BidDateTime,
                        BidderId = i.BidderId
                    }).ToList()
                })
                .ToListAsync();

            return items;
        }

        public async Task UpdateAsync(AuctionItem item)
        {
            // Update the auction item in the database
            _context.AuctionItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAuctionItem(int itemID)
        {
            // Update the auction item in the database
            var item = _context.AuctionItems.FindAsync(itemID).Result;
            item.Status = Status.Paid;
            _context.Entry(item).Property(x => x.Status).IsModified = true;
            await _context.SaveChangesAsync();
        }
    }
}
