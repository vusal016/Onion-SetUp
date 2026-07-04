namespace OnionSetUp.Domain.Exceptions
{
    public class ConflictException(string message):BaseException(message,409)
    {
    }
}