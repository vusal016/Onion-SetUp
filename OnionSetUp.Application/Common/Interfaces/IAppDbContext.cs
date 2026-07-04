namespace OnionSetUp.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<AppUser> AppUsers { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    }
}