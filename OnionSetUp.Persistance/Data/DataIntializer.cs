namespace OnionSetUp.Persistence.Data
{//Auto mig. ve seed data ile role gondermek ve admin yaratmaq.
    public class DataIntializer(AppDbContext dbContext, RoleManager<IdentityRole<Guid>> roleManager, UserManager<AppUser> userManager)
    {
        public async Task Intialize()
        {
            await dbContext.Database.MigrateAsync();
            await SeedRoles();
            await SeedAdmin();

        }
        private async Task SeedRoles()
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine,
                    result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                }
            }
        }
        private async Task SeedAdmin()
        {
            var adminEmail = "admin@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new AppUser("System Admin")
                {
                    UserName = "Admin",
                    Email = adminEmail,
                };
                var result = await userManager.CreateAsync(admin, "salam123");
                if (!result.Succeeded)
                {
                   throw new Exception(string.Join(Environment.NewLine,
                   result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                }
               var result1=await userManager.AddToRoleAsync(admin, "Admin");
                if (!result1.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine,
                    result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                }
            }
        }
    }
}
