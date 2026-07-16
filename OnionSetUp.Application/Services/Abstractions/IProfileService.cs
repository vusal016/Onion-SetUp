namespace OnionSetUp.Application.Services.Abstractions
{
    public interface IProfileService
    {
        Task<ProfileDto> MeAsync(Guid id,CancellationToken ct=default);
        Task<ProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto updateProfile, CancellationToken ct = default);
        Task ChangePasswordAsync(Guid id, ChangePasswordDto changePassword, CancellationToken ct = default);
        Task<string> UploadImageAsync(Guid id, IFormFile file, CancellationToken ct = default);
        Task DeleteImageAsync(Guid id, CancellationToken ct = default);
    }
}