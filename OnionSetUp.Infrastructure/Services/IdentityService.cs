namespace OnionSetUp.Infrastructure.Services
{
    public class IdentityService(UserManager<AppUser> userManager,IOptions<JwtSettings> jwtSettings) : IIdentityService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var existUser = await userManager.FindByEmailAsync(request.Email);
            if (existUser is not null)
                throw new InvalidOperationException(ErrorMessages.EmailAlreadyExists);
            var user = new AppUser(request.FullName)
            {
                Email=request.Email,
                UserName=request.UserName,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(ErrorMessages.InvalidCredentials);
            }
            var token = await GenerateJwtToken(user);
            return  BuildAuthResponse(user, token);
        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            var user = await userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException(ErrorMessages.Unauthorized);
            if (await userManager.IsLockedOutAsync(user)) throw new UnauthorizedAccessException(ErrorMessages.Unauthorized);
            var isPassWordValid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!isPassWordValid)
            {//AccesFailed parol sehv yazdiqda sayir 
                await userManager.AccessFailedAsync(user);
                throw new UnauthorizedAccessException(ErrorMessages.Unauthorized);
            }
            //Reset ise ugurlu girisde temizleyir
            await userManager.ResetAccessFailedCountAsync(user);
            var token = await GenerateJwtToken(user);
            return BuildAuthResponse(user,token);
        }
        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name,user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExExprationInMinutes),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private AuthResponseDto BuildAuthResponse(AppUser user, string token)


        {
            return new AuthResponseDto(
                Token:token,
                ExpiresAt:DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExExprationInMinutes),
                Email:user.Email,
                UserName:user.UserName
                );
        }
    }
}
