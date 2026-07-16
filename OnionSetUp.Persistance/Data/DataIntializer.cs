namespace OnionSetUp.Persistence.Data
{//Auto mig. ve seed data ile role gondermek ve admin yaratmaq.
//Burdaki error elave usulu developer ucun detallidir ona gore code ve s gosterilir service den ferqli olaraq.
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
            string[] roles = ["Admin","User"];
            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role)) continue;
                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                if (!result.Succeeded)
                    throw new Exception(string.Join(Environment.NewLine,
                    result.Errors.Select(e => $"{e.Code}: {e.Description}")));
            }
        }
        private async Task SeedAdmin()
        {
            var adminEmail = "admin@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new AppUser("System Admin",FilePaths.DefaultImage)
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
                var result1 = await userManager.AddToRoleAsync(admin, "Admin");
                if (!result1.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine,
                    result1.Errors.Select(e => $"{e.Code}: {e.Description}")));
                }
            }
        }   
    }
}