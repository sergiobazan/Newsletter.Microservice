using Microsoft.EntityFrameworkCore;
using Newsletter.Reporting.Api.Database;

namespace Newsletter.Reporting.Api.Extentions;

public static class MigrationExtentions
{
    public static void AddMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();
    }
}
