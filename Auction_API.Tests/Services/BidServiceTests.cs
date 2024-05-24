using Auction_API.Dto.Bid;
using Auction_API.Entities;
using Auction_API.Repositories.Base;
using Auction_API.Services;
using AutoMapper;
using FluentAssertions;
using NSubstitute;

namespace Auction_API.Tests.Service;

public class BidServiceTests
{
    private readonly IRepository<Bid> _bidRepository = Substitute.For<IRepository<Bid>>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly BidService _sut; 

    public BidServiceTests()
    {
        _sut = new BidService(_bidRepository, _mapper);
    }

    [Fact]
    public void GetById_ValidId_ReturnsBid()
    {
        // Arrange
        var bid = new Bid { AuctionItemId = 1, Amount = 100, BidderId = "testUser" };
        _bidRepository.GetById(1).Returns(bid);
        var bidDto = new BidSummaryDTO { AuctionItemId = 1, Amount = 100, BidderId = "testUser" };
        _mapper.Map<BidSummaryDTO>(bid).Returns(bidDto);

        // Act
        var result = _sut.GetById(1);

        // Assert
        result.Should().BeEquivalentTo(bidDto);
    }

    [Fact]
    public async Task PlaceBidAsync_ValidBid_ReturnsTrue()
    {
        // Arrange
        var bidDto = new BidDTO { ItemId = 1, Amount = 100 }; 
        var bid = new Bid { AuctionItemId = 1, Amount = 100, BidderId = "testUser" };
        _mapper.Map<Bid>(bidDto).Returns(bid);
        _bidRepository.PlaceBidAsync(bid, "testUser").Returns(Task.FromResult(true));

        // Act
        var result = await _sut.PlaceBidAsync(bidDto, "testUser");

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task PlaceBidAsync_BidLowerThanCurrentHighestBid_ReturnsFalse()
    {
        // Arrange
        var bidDto = new BidDTO { ItemId = 1, Amount = 50 }; 
        var bid = new Bid { AuctionItemId = 1, Amount = 50, BidderId = "testUser" };
        _mapper.Map<Bid>(bidDto).Returns(bid);
        _bidRepository.PlaceBidAsync(bid, "testUser").Returns(Task.FromResult(false)); 

        // Act
        var result = await _sut.PlaceBidAsync(bidDto, "testUser");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task PlaceBidAsync_ItemAlreadySoldOrCancelled_ReturnsFalse()
    {
        // Arrange
        var bidDto = new BidDTO { ItemId = 1, Amount = 100 }; 
        var bid = new Bid { AuctionItemId = 1, Amount = 100, BidderId = "testUser" };
        _mapper.Map<Bid>(bidDto).Returns(bid);
        _bidRepository.PlaceBidAsync(bid, "testUser").Returns(Task.FromResult(false));

        // Act
        var result = await _sut.PlaceBidAsync(bidDto, "testUser");

        // Assert
        result.Should().BeFalse();
    }
}