using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auction_API.Dto.Category;
using Auction_API.Entities;
using Auction_API.Repositories.Base;
using Auction_API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auction_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        
        [HttpGet("")]
        public ActionResult GetListCategories()
        {
            return Ok(_categoryService.GetAllCategories());
        }
    }
}
