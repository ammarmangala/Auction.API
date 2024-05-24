using Auction_API.Dto.Category;
using Auction_API.Entities;
using AutoMapper;

namespace Auction_API.Profiles;

public class CategoryDTOProfile : Profile
{
    public CategoryDTOProfile()
    {
        CreateMap<Category, CategoryDTO>();
    }
}