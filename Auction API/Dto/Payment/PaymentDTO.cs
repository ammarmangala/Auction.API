namespace Auction_API.Dto.Payment
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } // The ID of the user who is making the payment
        public int AuctionItemId { get; set; } // The ID of the auction item for which the payment is being made
    }
}
