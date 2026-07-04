namespace OnionSetUp.Domain.Common
{
    public class BaseException(string message,int statusCode) : Exception(message)
    {
        public int StatusCode { get; init; } = statusCode;  
    }
}
