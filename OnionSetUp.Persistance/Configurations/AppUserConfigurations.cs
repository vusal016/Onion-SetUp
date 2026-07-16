namespace OnionSetUp.Persistence.Configurations
{
    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}