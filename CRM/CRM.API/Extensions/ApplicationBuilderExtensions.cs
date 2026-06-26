using CRM.Infrastructure.Identity;

namespace CRM.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDatabaseAsync(
        this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var seeder =
            scope.ServiceProvider
                .GetRequiredService<IdentitySeeder>();

        await seeder.SeedAsync();
    }
}