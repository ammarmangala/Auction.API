namespace Auction_API.Services
{
    public interface IUserService
    {
        Task<string> GetSubscriptionTypeAsync(string userId);
    }
}
