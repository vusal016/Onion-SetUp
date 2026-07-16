namespace OnionSetUp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken ct)
        {
            var result = await userService.GetAllUsersAsync(ct);
            var response = Response<List<UserDto>>.Success(result, 200);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id,CancellationToken ct)
        {
            var result = await userService.GetUserByIdAsync(id, ct);
            var response = Response<UserDto>.Success(result, 200);
            return Ok(response);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> ChangeRole(Guid id,ChangeRoleRequestDto roleRequest, CancellationToken ct)
        {
            var result = await userService.ChangeRoleAsync(id, roleRequest, ct);
            var response = Response<UserDto>.Success(result, 200);
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
        {
            await userService.DeleteUserAsync(id, ct);
            var response = Response<string>.Success(200);
            return Ok(response);
        }
    }
}