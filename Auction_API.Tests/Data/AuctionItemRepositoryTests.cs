using System.Data.Common;
using Auction_API.Data;
using Auction_API.Dto.AuctionItem;
using Auction_API.Entities;
using Auction_API.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Auction_API.Tests.Data;

public class AuctionItemRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> _options;

    public AuctionItemRepositoryTests()
    {
        DbConnection connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection).Options;

        ApplicationDbContext dbContext = CreateDbContext();
        dbContext.Database.EnsureCreated();
    }

    private ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext(_options);
    }

    [Fact]
    public async Task GetSoldItemsByUserIdAsync_UserIdWithSoldItems_ReturnsListOfSoldItems()
    {
        // Arrange
        ApplicationDbContext dbContext = CreateDbContext();
        AuctionItemRepository sut = new AuctionItemRepository(dbContext);
        string userId = "testUserId";
        var category = new Category { Name = "Test Category" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        var auctionItems = new List<AuctionItem>
        {
            new AuctionItem { Name = "Test Item 1", Description = "Test Description 1", SellerId = userId, Status = Status.Paid, CategoryId = category.Id },
            new AuctionItem { Name = "Test Item 2", Description = "Test Description 2", SellerId = userId, Status = Status.Paid, CategoryId = category.Id },
            new AuctionItem { Name = "Test Item 3", Description = "Test Description 3", SellerId = "otherUserId", Status = Status.Paid, CategoryId = category.Id }
        };
        dbContext.AuctionItems.AddRange(auctionItems);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetSoldItemsByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.All(i => i.SellerId == userId && i.Status == Status.Paid).Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsAuctionItem()
    {
        // Arrange
        ApplicationDbContext dbContext = CreateDbContext();
        AuctionItemRepository sut = new AuctionItemRepository(dbContext);
        var category = new Category { Name = "Test Category" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        var auctionItem = new AuctionItem { Name = "Test Item", Description = "Test Description", SellerId = "Test SellerId", CategoryId = category.Id }; 
        dbContext.AuctionItems.Add(auctionItem);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetByIdAsync(auctionItem.Id);

        // Assert
        var expectedAuctionItem = new AuctionItem { Id = auctionItem.Id, Name = "Test Item", Description = "Test Description", SellerId = "Test SellerId", CategoryId = category.Id, Category = new Category { Id = category.Id, Name = "Test Category" } }; 
        result.Should().BeEquivalentTo(expectedAuctionItem, options => options.Excluding(a => a.Category.AuctionItems).Excluding(a => a.Bids)); 
    }

    [Fact]
    public async Task SearchItemsAsync_ValidSearchCriteria_ReturnsMatchingItems()
    {
        // Arrange
        ApplicationDbContext dbContext = CreateDbContext();
        AuctionItemRepository sut = new AuctionItemRepository(dbContext);
        var auctionItems = new List<AuctionItem>
        {
            new AuctionItem { Name = "Test Item 1", Description = "Description 1", SellerId = "Seller1", CategoryId = 1, StartingPrice = 50, Status = Status.Initial, EndDateTime = DateTime.Now.AddDays(1) },
            new AuctionItem { Name = "Test Item 2", Description = "Description 2", SellerId = "Seller2", CategoryId = 1, StartingPrice = 150, Status = Status.Initial, EndDateTime = DateTime.Now.AddDays(1) },
            new AuctionItem { Name = "Test Item 3", Description = "Description 3", SellerId = "Seller3", CategoryId = 2, StartingPrice = 50, Status = Status.Initial, EndDateTime = DateTime.Now.AddDays(1) }
        };
        dbContext.AuctionItems.AddRange(auctionItems);
        await dbContext.SaveChangesAsync();
        var searchDto = new AuctionItemSearchDTO { Categories = new List<int> { 1 }, MaxPrice = 100 };

        // Act
        var result = await sut.SearchItemsAsync(searchDto.Categories, searchDto.MaxPrice);

        // Assert
        result.Should().HaveCount(1);
        result.All(i => i.CategoryId == 1 && i.StartingPrice <= 100).Should().BeTrue();
    }

    [Fact]
    public async Task GetPurchasedItemsByUserIdAsync_UserIdWithPurchasedItems_ReturnsListOfPurchasedItems()
    {
        // Arrange
        ApplicationDbContext dbContext = CreateDbContext();
        AuctionItemRepository sut = new AuctionItemRepository(dbContext);
        string userId = "testUserId";
        var category = new Category { Name = "Test Category" };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();
        
        var auctionItems = new List<AuctionItem>
        {
            new AuctionItem { Name = "Test Item 1", Description = "Test Description 1", SellerId = userId, Status = Status.Paid, CategoryId = category.Id },
            new AuctionItem { Name = "Test Item 2", Description = "Test Description 2", SellerId = userId, Status = Status.Paid, CategoryId = category.Id },
            new AuctionItem { Name = "Test Item 3", Description = "Test Description 3", SellerId = "otherUserId", Status = Status.Paid, CategoryId = category.Id }
        };
        dbContext.AuctionItems.AddRange(auctionItems);
        await dbContext.SaveChangesAsync();

        var bids = new List<Bid>
        {
            new Bid { AuctionItemId = auctionItems[0].Id, BidderId = userId, Amount = 200 },
            new Bid { AuctionItemId = auctionItems[1].Id, BidderId = userId, Amount = 300 },
            new Bid { AuctionItemId = auctionItems[2].Id, BidderId = "otherUserId", Amount = 400 }
        };
        dbContext.Bids.AddRange(bids);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetPurchasedItemsByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2);
        result.All(i => i.SellerId == userId && i.Status == Status.Paid).Should().BeTrue();
    }
}