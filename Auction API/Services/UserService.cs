using Microsoft.AspNetCore.Identity;

namespace Auction_API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetSubscriptionTypeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var claims = await _userManager.GetClaimsAsync(user);
            var subscriptionClaim = claims.FirstOrDefault(c => c.Type == "SubscriptionType");

            // Default Free voor een user 
            return subscriptionClaim?.Value ?? "Free";
        }
    }
}
