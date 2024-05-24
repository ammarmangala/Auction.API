using Auction_API.Dto.Bid;
using Auction_API.Entities;
using Auction_API.Repositories.Base;
using AutoMapper;

namespace Auction_API.Services;

public class BidService : IBidService
{
    private IRepository<Bid> _bidService;
    private IMapper _mapper;
    
    public BidService(IRepository<Bid> bidService, IMapper mapper)
    {
        _bidService = bidService;
        _mapper = mapper;
    }
    public BidSummaryDTO GetById(int id)
    {
        Bid bids = _bidService.GetById(id);

        if (bids == null)
        {
            return null;
        }

        return _mapper.Map<BidSummaryDTO>(bids);
    }
    
    
    public async Task<bool> PlaceBidAsync(BidDTO dto, string userId)
    {
        var bid = _mapper.Map<Bid>(dto);
        bid.AuctionItemId = dto.ItemId;
        return await _bidService.PlaceBidAsync(bid, userId);
        return true;
    }
    
}