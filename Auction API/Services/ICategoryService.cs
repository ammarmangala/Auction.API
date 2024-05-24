using Auction_API.Dto.Category;

namespace Auction_API.Services;

public interface ICategoryService
{
    IEnumerable<CategoryDTO> GetAllCategories();
}