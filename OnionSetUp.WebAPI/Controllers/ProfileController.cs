namespace OnionSetUp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpPatch("Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePassword, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await profileService.ChangePasswordAsync(Guid.Parse(userId!), changePassword, ct);
            return Ok(Response<string>.Success("Password Changed Successfully", 200));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await profileService.DeleteImageAsync(Guid.Parse(userId!), ct);
            return Ok(Response<string>.Success("Image Deleted Successfully", 200));
        }
        [HttpGet("Me")]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userId =User.FindFirstValue(ClaimTypes.NameIdentifier );
            var result = await profileService.MeAsync(Guid.Parse(userId!), ct);
            var response = Response<ProfileDto>.Success(result, 200);
            return Ok(response);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto updateProfile, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await profileService.UpdateProfileAsync(Guid.Parse(userId!), updateProfile, ct);
            var response = Response<ProfileDto>.Success(result, 200);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await profileService.UploadImageAsync(Guid.Parse(userId!), file, ct);
            var response = Response<string>.Success(result, 200);
            return Ok(response);
        }
    }
}