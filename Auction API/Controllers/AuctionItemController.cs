using System.Security.Claims;
using Auction_API.Dto.AuctionItem;
using Auction_API.Dto.Bid;
using Auction_API.Extension;
using Auction_API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Auction_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionItemController : ControllerBase
    {
        private readonly IAuctionItemService _auctionItemService;
        private readonly IBidService _bidService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuctionItemController> _logger;

        public AuctionItemController(IAuctionItemService auctionItemService, IBidService bidService, ILogger<AuctionItemController> logger,
            IMapper mapper)
        {
            _auctionItemService = auctionItemService;
            _bidService = bidService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("items")]
        public async Task<IActionResult> CreateAuctionItem(CreateAuctionItemDTO dto)
        {
            var userId = User.GetUserId();
            var auctionItem = await _auctionItemService.CreateAuctionItem(dto, userId);
            
            return CreatedAtAction(nameof(GetAuctionItem), new { itemId = auctionItem.Id }, auctionItem);
        }

        [HttpDelete("items/{itemId}/cancel")]
        public async Task<IActionResult> CancelAuctionItem(int itemId)
        {
            var userId = User.GetUserId();
            var success = await _auctionItemService.CancelAuctionItem(itemId, userId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("sellers/{userId}/items")]
        public async Task<IActionResult> GetSoldItems(string userId)
        {
            var currentUserId = User.GetUserId();
            if (userId != currentUserId)
            {
                return Unauthorized();
            }
            var soldItems = await _auctionItemService.GetSoldItems(userId);
            return Ok(soldItems);
        }
        
        [HttpGet("items/{itemId}/biddings")]
        public async Task<IActionResult> GetBidsForItem(int itemId)
        {
            var userId = User.GetUserId();
            var auctionItem = await _auctionItemService.GetAuctionItemById(itemId);
            if (auctionItem == null)
            {
                return NotFound();
            }

            if (auctionItem.SellerId != userId)
            {
                return Forbid();
            }

            var bids = await _auctionItemService.GetBidsForItem(itemId);
            return Ok(bids);
        }


        [HttpGet("items/{itemId}")]
        public async Task<IActionResult> GetAuctionItem(int itemId)
        {
            var auctionItem = await _auctionItemService.GetAuctionItemById(itemId);
            if (auctionItem == null)
            {
                return NotFound();
            }
            return Ok(auctionItem);
        }


        [HttpGet("items/search")]
        public async Task<IActionResult> SearchItems([FromQuery] AuctionItemSearchDTO dto)
        {
            var items = await _auctionItemService.SearchItemsAsync(dto);
            return Ok(items);
        }


        [HttpPost("items/{itemId}/bids")]
        public async Task<IActionResult> PlaceBid(int itemId, [FromBody] BidDTO dto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user id from the claims
            if (userId == null)
            {
                return BadRequest("User identifier is missing.");
            }

            dto.ItemId = itemId; // Set the itemId from the route
            var result = await _bidService.PlaceBidAsync(dto, userId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Unable to place bid");
            }
        }

        [HttpGet("buyers/{userId}/items")]
        public async Task<IActionResult> GetPurchasedItems(string userId)
        {
            var currentUserId = User.GetUserId(); 
            if (userId != currentUserId)
            {
                return Unauthorized();
            }
            var items = await _auctionItemService.GetPurchasedItemsByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpPost("buyers/{userId}/items/{itemId}/payment")]
        public async Task<IActionResult> SimulatePayment(string userId, int itemId)
        {
            var result = await _auctionItemService.SimulatePaymentAsync(itemId, userId);
            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound("Unable to simulate payment.");
            }
        }
    }
}
