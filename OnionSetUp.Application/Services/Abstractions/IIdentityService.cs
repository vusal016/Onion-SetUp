namespace OnionSetUp.Application.Services.Abstractions
{
    public interface IIdentityService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default);
        Task LogoutAsync();
    }
}
