using Microsoft.EntityFrameworkCore;
using Polly;

namespace Contact.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static bool IsInKubernetes(this IApplicationBuilder applicationBuilder)
    {
        var config = applicationBuilder.ApplicationServices.GetRequiredService<IConfiguration>();
        var orchestratorType = config.GetValue<string>("OrchestratorType");
        return orchestratorType?.ToUpper() == "K8S";
    }

    public static IApplicationBuilder MigrateDbContext<TContext>(this IApplicationBuilder applicationBuilder, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
    {
        var isUnderKubernetes = applicationBuilder.IsInKubernetes();

        using var scope = applicationBuilder.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;

        var context = services.GetService<TContext>();

        try
        {
            if (isUnderKubernetes)
            {
                InvokeSeeder(seeder, context, services);
            }
            else
            {
                var retry = Policy.Handle<Exception>()
                    .WaitAndRetry(new[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8)
                    });

                retry.Execute(() => InvokeSeeder(seeder, context, services));
            }
        }
        catch (Exception e)
        {
            if (isUnderKubernetes)
                throw;
        }

        return applicationBuilder;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
        where TContext : DbContext
    {
        context.Database.Migrate();
        seeder(context, services);
    }
}