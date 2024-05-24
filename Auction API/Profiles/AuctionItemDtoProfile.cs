using Auction_API.Dto.AuctionItem;
using Auction_API.Dto.Category;
using Auction_API.Entities;
using AutoMapper;
namespace Auction_API.Profiles
{
    public class AuctionItemDtoProfile : Profile
    {
        public AuctionItemDtoProfile()
        {
            CreateMap<AuctionItem, AuctionItemDTO>();
            CreateMap<AuctionItemDTO, AuctionItem>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<CreateAuctionItemDTO, AuctionItem>();
        }
    }
}
