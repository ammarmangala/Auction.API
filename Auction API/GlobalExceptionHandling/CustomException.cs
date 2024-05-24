namespace Auction_API.GlobalExceptionHandling
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }
    }
}
