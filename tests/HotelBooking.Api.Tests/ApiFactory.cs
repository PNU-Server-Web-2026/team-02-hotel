using HotelBooking.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace HotelBooking.Api.Tests;

/// <summary>
/// Фабрика тестового сервера для інтеграційних тестів.
/// Піднімає справжній PostgreSQL у Docker-контейнері (Testcontainers),
/// застосовує міграції та підміняє рядок підключення API на адресу контейнера.
/// Один екземпляр на весь прогін тестів — див. <see cref="IntegrationTestCollection"/>.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _db =
        new PostgreSqlBuilder("postgres:17-alpine").Build();

    /// <summary>
    /// Викликається xUnit один раз перед першим тестом колекції.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _db.StartAsync();

        // Звернення до Services створює тестовий сервер. Контейнер на цей момент
        // уже запущений, тож ConfigureWebHost отримає правильний рядок підключення.
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", _db.GetConnectionString());
    }

    /// <summary>
    /// Очищає всі таблиці схеми public, крім історії міграцій,
    /// щоб тести не залежали один від одного і від порядку запуску.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_db.GetConnectionString());
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            DO $$
            DECLARE tables text;
            BEGIN
                SELECT string_agg(format('%I.%I', schemaname, tablename), ', ')
                INTO tables
                FROM pg_tables
                WHERE schemaname = 'public' AND tablename <> '__EFMigrationsHistory';

                IF tables IS NOT NULL THEN
                    EXECUTE 'TRUNCATE ' || tables || ' RESTART IDENTITY CASCADE';
                END IF;
            END $$;
            """;
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Викликається xUnit після останнього тесту: зупиняє сервер і видаляє контейнер.
    /// Реалізовано явно, бо WebApplicationFactory вже має DisposeAsync з іншим типом (ValueTask).
    /// </summary>
    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await _db.DisposeAsync();
    }
}
