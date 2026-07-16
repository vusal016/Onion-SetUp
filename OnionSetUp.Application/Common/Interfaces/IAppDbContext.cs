namespace OnionSetUp.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<AppUser> AppUsers { get; set; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }
        DbSet<IdentityRole<Guid>> Roles { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    }
}