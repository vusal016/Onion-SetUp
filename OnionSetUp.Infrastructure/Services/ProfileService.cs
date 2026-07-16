namespace OnionSetUp.Infrastructure.Services
{
    public class ProfileService(UserManager<AppUser> userManager, IFileStorageService fileStorage, IMapper mapper) : IProfileService
    {
        public async Task ChangePasswordAsync(Guid id, ChangePasswordDto changePassword, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            var result = await userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"{ErrorMessages.InvalidCredentials}. Details: {errors}");
            }
        }

        public async Task DeleteImageAsync(Guid id, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            if (user.ImageUrl != FilePaths.DefaultImage)
            {
                user.UpdateImageUrl(FilePaths.DefaultImage);
                await userManager.UpdateAsync(user);
            }
        }
        public async Task<ProfileDto> MeAsync(Guid id, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            var dto = mapper.Map<ProfileDto>(user);
            dto.Roles.AddRange(await userManager.GetRolesAsync(user));
            return dto;
        }
        public async Task<ProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto updateProfile, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
             ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            if (updateProfile.FullName.Length < 3)
                throw new BadRequestException(ErrorMessages.InvalidInput);
            user.UpdateFullName(updateProfile.FullName);
            await userManager.UpdateAsync(user);
            var dto = mapper.Map<ProfileDto>(user);
            dto.Roles.AddRange(await userManager.GetRolesAsync(user));
            return dto;
        }

        public async Task<string> UploadImageAsync(Guid id, IFormFile file, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
           ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            var url = await fileStorage.UploadImageAsync(file, FilePaths.UserFolder);
            user.UpdateImageUrl(url);
            await userManager.UpdateAsync(user);
            return url;
        }
    }
}