namespace Auction_API.Dto.Authentication;

public class RegisterUserDTO
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string SubscriptionType { get; set; } // Free, Gold, Platinum
}