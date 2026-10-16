using HotelBooking.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBooking.Api.Tests.Infrastructure;

/// <summary>
/// Перевіряє саму тестову інфраструктуру: API підключений до справжнього
/// PostgreSQL з контейнера, а ResetDatabaseAsync очищає дані між тестами.
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class DatabaseTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task Api_UsesRealPostgreSql()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.True(await context.Database.CanConnectAsync());
        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", context.Database.ProviderName);
    }

    [Fact]
    public async Task ResetDatabase_RemovesAllRows()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.ExecuteSqlRawAsync(
            "CREATE TABLE IF NOT EXISTS reset_probe (id serial PRIMARY KEY, value text)");
        await context.Database.ExecuteSqlRawAsync(
            "INSERT INTO reset_probe (value) VALUES ('a'), ('b')");

        await Factory.ResetDatabaseAsync();

        var count = await context.Database
            .SqlQueryRaw<int>("SELECT COUNT(*)::int AS \"Value\" FROM reset_probe")
            .SingleAsync();
        Assert.Equal(0, count);

        await context.Database.ExecuteSqlRawAsync("DROP TABLE reset_probe");
    }
}
