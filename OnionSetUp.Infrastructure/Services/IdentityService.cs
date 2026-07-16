namespace OnionSetUp.Infrastructure.Services
{
    public class IdentityService(UserManager<AppUser> userManager, IFileStorageService fileStorage, IOptions<JwtSettings> jwtSettings) : IIdentityService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var existUser = await userManager.FindByEmailAsync(request.Email);
            if (existUser is not null)
                throw new InvalidOperationException(ErrorMessages.EmailAlreadyExists);
            var user = new AppUser(request.FullName, FilePaths.DefaultImage)
            {
                Email = request.Email,
                UserName = request.Email,
            };
            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"{ErrorMessages.InvalidCredentials}. Details: {errors}");
            }
            var addRole = await userManager.AddToRoleAsync(user, "User");
            if (!addRole.Succeeded)
            {
                var errors = string.Join(", ", addRole.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"{ErrorMessages.OperationFailed}. Details: {errors}");
            }
            var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpirationInMinutes);
            var token = await GenerateJwtToken(user, expiresAt);
            return BuildAuthResponse(user, token, expiresAt);
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
            var expiresAt = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpirationInMinutes);
            var token = await GenerateJwtToken(user, expiresAt);
            return BuildAuthResponse(user, token, expiresAt);
        }
        public Task LogoutAsync() => Task.CompletedTask;
        private async Task<string> GenerateJwtToken(AppUser user, DateTime expiresAt)
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
                expires: expiresAt,
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private AuthResponseDto BuildAuthResponse(AppUser user, string token, DateTime expiresAt)
        {
            return new AuthResponseDto(
                Token: token,
                ExpiresAt: expiresAt,
                Email: user.Email!,
                FullName: user.FullName!,
                ImageUrl: user.ImageUrl
                );
        }
    }
}