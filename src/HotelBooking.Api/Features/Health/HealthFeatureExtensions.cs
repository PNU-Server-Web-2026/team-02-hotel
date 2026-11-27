using HotelBooking.Api.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HotelBooking.Api.Features.Health;

public static class HealthFeatureExtensions
{
    private const string ReadyTag = "ready";

    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(name: "database", tags: [ReadyTag]);

        return services;
    }

    public static WebApplication MapHealthFeature(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = static _ => false,
            ResponseWriter = WriteJson,
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = static check => check.Tags.Contains(ReadyTag),
            ResponseWriter = WriteJson,
        });

        return app;
    }

    private static Task WriteJson(HttpContext context, HealthReport report)
    {
        var body = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
            }),
        };

        return context.Response.WriteAsJsonAsync(body);
    }
}
