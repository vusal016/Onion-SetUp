namespace OnionSetUp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IIdentityService _identService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto request, CancellationToken ct = default)
        {
            var result = await _identService.RegisterAsync(request);
            var response = Response<AuthResponseDto>.Success(result, 201);
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest, CancellationToken ct = default)
        {
            var result = await _identService.LoginAsync(loginRequest);
            var response = Response<AuthResponseDto>.Success(result, 200);
            return Ok(response);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _identService.LogoutAsync();
            var response = Response<string>.Success("Logout Successfull", 200);
            return Ok(response);
        }
    }
}
