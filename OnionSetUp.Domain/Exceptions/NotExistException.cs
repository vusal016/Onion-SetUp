namespace OnionSetUp.Domain.Exceptions
{
    public class NotExistException(string message):BaseException(message,404)
    {
    }
}
