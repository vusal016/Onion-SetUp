using OnionSetUp.WebAPI.Middlewares;

namespace OnionSetUp.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            // Ona göre burada scope yaradiriq ki, bu proqram baslayanda islemeli olan bir automigration-dur.
            // Program baslayanda ise hele heç bir request gelmediyi üçün scoped istifade ede bilmirik.
            // Buna göre de biz proqram start olan anda sanki request gelmis kimi manual bir scope yaradiriq,
            // sonra bu scope vasitesile initializer-i elde edib isledirik.
            using var scope = app.Services.CreateScope();
            var dataIntializer = scope.ServiceProvider.GetRequiredService<DataIntializer>();
            await dataIntializer.Intialize();
            //ExceptionMiddleware
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}