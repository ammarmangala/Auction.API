using Xunit;
using NSubstitute;
using Auction_API.Services;
using Auction_API.Repositories;
using Auction_API.Dto.AuctionItem;
using Auction_API.Entities;
using FluentAssertions;
using Auction_API.Data;
using Auction_API.GlobalExceptionHandling;
using Microsoft.EntityFrameworkCore;

namespace Auction_API.Tests.Service
{
    public class AuctionItemServiceTests
    {
        private readonly IAuctionItemRepository _auctionItemRepository;
        private readonly IUserService _userService;
        private readonly AuctionItemService _sut; 

        public AuctionItemServiceTests()
        {
            _auctionItemRepository = Substitute.For<IAuctionItemRepository>();
            _userService = Substitute.For<IUserService>();
            _sut = new AuctionItemService(_auctionItemRepository, _userService, null);
        }

        [Fact]
        public async Task CreateAuctionItem_AllDataCorrectForFreeUser_ReturnsAuctionItem()
        {
            // Arrange
            var userId = "testUserId";
            var dto = new CreateAuctionItemDTO
            {
                Name = "Test Item",
                Description = "Test Description",
                StartingPrice = 100,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddDays(3),
                CategoryId = 1
            };
            _userService.GetSubscriptionTypeAsync(userId).Returns("Free");
            _auctionItemRepository.AddAsync(Arg.Any<AuctionItem>()).Returns(new AuctionItem());

            // Act
            var result = await _sut.CreateAuctionItem(dto, userId);

            // Assert
            result.Should().NotBeNull();
            await _auctionItemRepository.Received().AddAsync(Arg.Any<AuctionItem>());
        }

        [Fact]
        public async Task CreateAuctionItem_InvalidEndTimeForGoldUser_ThrowsException()
        {
            // Arrange
            var userId = "testUserId";
            var dto = new CreateAuctionItemDTO
            {
                Name = "Test Item",
                Description = "Test Description",
                StartingPrice = 100,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(10), 
                CategoryId = 1
            };
            _userService.GetSubscriptionTypeAsync(userId).Returns("Gold");

            // Act
            var act = async () => { await _sut.CreateAuctionItem(dto, userId); };

            // Assert
            await act.Should().ThrowAsync<CustomException>().WithMessage("The end time must be at least 12 hours after the start time for Gold or Platinum users.");
        }
        
        [Fact]
        public async Task CancelAuctionItem_AttemptToCancelOthersItem_ThrowsException()
        {
            // Arrange
            var itemId = 1;
            var userId = "testUserId";
            var otherUserId = "otherUserId";
            var auctionItem = new AuctionItemDTO
            {
                Id = itemId,
                SellerId = otherUserId 
            };
            _auctionItemRepository.GetByIdAsync(itemId).Returns(Task.FromResult(auctionItem));

            // Create options for InMemory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:") 
                .Options;

            
            await using (var context = new ApplicationDbContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add entities to database
                if (!context.Categories.Any(c => c.Id == 1))
                {
                    context.Categories.Add(new Category { Id = 1, Name = "Test Category" });
                }
                context.AuctionItems.Add(new AuctionItem { Id = itemId, SellerId = otherUserId, Name = "Test Item", Description = "Test Description", CategoryId = 1 });
                context.SaveChanges();

               
                var service = new AuctionItemService(_auctionItemRepository, _userService, context);

                // Act
                var act = async () => { await service.CancelAuctionItem(itemId, userId); };

                // Assert
                await act.Should().ThrowAsync<CustomException>().WithMessage("Only the seller can cancel the sale.");
            }
        }
        
        [Fact]
        public async Task GetSoldItems_ReturnsOnlyItemsSoldByUser()
        {
            // Arrange
            var userId = "testUserId";
            var otherUserId = "otherUserId";

            var soldItems = new List<AuctionItemDTO>
            {
                new AuctionItemDTO { Id = 1, SellerId = userId, Status = Status.Paid },
                new AuctionItemDTO { Id = 2, SellerId = userId, Status = Status.Paid },
                new AuctionItemDTO { Id = 3, SellerId = otherUserId, Status = Status.Paid },
                new AuctionItemDTO { Id = 4, SellerId = userId, Status = Status.Initial }, 
            };

            _auctionItemRepository.GetSoldItemsByUserIdAsync(userId).Returns(soldItems.Where(i => i.SellerId == userId && i.Status == Status.Paid));

            // Act
            var result = await _sut.GetSoldItems(userId);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.All(i => i.SellerId == userId && i.Status == Status.Paid).Should().BeTrue();
        }
    }
}