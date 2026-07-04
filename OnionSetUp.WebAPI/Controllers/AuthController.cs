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
            var response = Response<AuthResponseDto>.Succes(result,201);
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest, CancellationToken ct = default)
        {
            var result = await _identService.LoginAsync(loginRequest);
            var response = Response<AuthResponseDto>.Succes(result, 200);
            return Ok(response);
        }
        [HttpGet("Me")]
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { email, name });
        }
    }
}
