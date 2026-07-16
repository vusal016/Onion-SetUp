namespace OnionSetUp.Domain.Exceptions
{
    public class UserNotFoundException(string message):BaseException(message,404)
    {
    }
}
