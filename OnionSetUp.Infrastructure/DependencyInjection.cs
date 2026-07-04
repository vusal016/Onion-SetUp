using OnionSetUp.Infrastructure.Services;

namespace OnionSetUp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //JwtSettings i c# obyekte ceviririk
            var jwtSettings = configuration.GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>();
            // Authentication middleware-ni əlavə edirik.
            // Gələn hər HTTP request-də istifadəçi doğrulanacaq.
            services.AddAuthentication(options =>
            {    // Authenticate zamanı istifadə olunacaq standart sxem JWT Bearer olacaq.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                // İstifadəçi doğrulanmasa Challenge (401 Unauthorized) JWT Bearer tərəfindən idarə olunacaq.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JWT Bearer Authentication-u əlavə edirik.
            .AddJwtBearer(options =>
            {      // Token yoxlanışı üçün istifadə olunan qaydalar.
                options.TokenValidationParameters = new TokenValidationParameters()
                { // Tokenin imzası yoxlanılsın.
                    ValidateIssuerSigningKey = true,
                    // Token hansı SecretKey ilə imzalanıbsa, həmin açarla yoxlanılacaq.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    // Tokeni yaradan (Issuer) yoxlanılsın.
                    ValidateIssuer = true,
                    // Gözlənilən Issuer.
                    ValidIssuer = jwtSettings.Issuer,
                    // Token hansı tətbiq üçün yaradılıb (Audience) yoxlanılsın.
                    ValidateAudience = true,
                    // Gözlənilən Audience.
                    ValidAudience = jwtSettings.Audience,
                    // Tokenin vaxtı bitibsə qəbul edilməsin.
                    ValidateLifetime = true,
                    // Əlavə vaxt güzəşti verilməsin.
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            return services;
        }
    }
}
