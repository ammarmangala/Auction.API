using Auction_API.Dto.Bid;
using Auction_API.Entities;
using AutoMapper;

namespace Auction_API.Profiles;

public class BidDTOProfile : Profile
{
    public BidDTOProfile()
    {
        CreateMap<Bid, BidDTO>();
        CreateMap<BidDTO, Bid>();
        CreateMap<Bid, BidSummaryDTO>();
    }
}