namespace OnionSetUp.Infrastructure.Services
{
    public class UserService(IAppDbContext appDb, UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper) : IUserService
    {
        public async Task<UserDto> ChangeRoleAsync(Guid id, ChangeRoleRequestDto roleRequest, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);

            if (!await roleManager.RoleExistsAsync(roleRequest.NewRole))
                throw new NotExistException(ErrorMessages.RoleNotFound);

            var roles = await userManager.GetRolesAsync(user);

            var remove = await userManager.RemoveFromRolesAsync(user, roles);
            if (!remove.Succeeded)
            {
                var errors = string.Join(',', remove.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"{ErrorMessages.OperationFailed}. Details: {errors}");
            }
            var result = await userManager.AddToRoleAsync(user, roleRequest.NewRole);
            if (!result.Succeeded)
            {
                var errors = string.Join(',', result.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"{ErrorMessages.InvalidOperation}. Details: {errors}");
            }
            var dto = mapper.Map<UserDto>(user);
            dto.Roles.Add(roleRequest.NewRole);
          
            return dto;
        }
        public async Task DeleteUserAsync(Guid id, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            var deleted = await userManager.DeleteAsync(user);
            if (!deleted.Succeeded)
            {
                var errors = string.Join(',', deleted.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"{ErrorMessages.OperationFailed}. Details:{errors}");
            }
        }
        public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken ct = default)
        {
            var users = await (
                from user in appDb.AppUsers
                select new UserDto(
                user.Id,
                user.FullName,
                user.Email!,
              (from ur in appDb.UserRoles
               join r in appDb.Roles
               on ur.RoleId equals r.Id
               where ur.UserId == user.Id
               select r.Name!).ToList())).ToListAsync(ct);

            return users;
        }
        public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new UserNotFoundException(ErrorMessages.UserNotFound);
            var dto = mapper.Map<UserDto>(user);
            dto.Roles.AddRange(await userManager.GetRolesAsync(user));
            
            return dto;
        }
    }
}