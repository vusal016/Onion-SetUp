namespace OnionSetUp.Application.Services.Abstractions
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct = default);
        Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<UserDto> ChangeRoleAsync(Guid id,ChangeRoleRequestDto roleRequest,CancellationToken ct = default);
        Task DeleteUserAsync(Guid id, CancellationToken ct = default);
    }
}
