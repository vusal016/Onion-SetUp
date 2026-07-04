namespace OnionSetUp.Application.Common.Identity
{
    public record AuthResponseDto(string Token,DateTime ExpiresAt,string Email,string UserName);
}
