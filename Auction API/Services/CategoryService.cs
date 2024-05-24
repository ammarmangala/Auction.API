using Auction_API.Dto.Category;
using Auction_API.Entities;
using Auction_API.Repositories.Base;
using AutoMapper;

namespace Auction_API.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    
    public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    
    public IEnumerable<CategoryDTO> GetAllCategories()
    {
           return _categoryRepository.GetAll().Select(_mapper.Map<CategoryDTO>);
    }
}