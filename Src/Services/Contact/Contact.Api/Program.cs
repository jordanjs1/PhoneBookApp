using Contact.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Contact.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddConfiguration(configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddEntityFrameworkNpgsql().AddDbContext<BookContext>(options =>
            {
                options.UseNpgsql(configuration["ConnectionString"], pgSqlOptions =>
                {
                    pgSqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                    pgSqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
            });

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            /* The following code can be used to apply migrations on runtime. Disabled to prevent database corruption.
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BookContext>();
                dbContext.Database.Migrate();
            }
            */

            app.Run();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}