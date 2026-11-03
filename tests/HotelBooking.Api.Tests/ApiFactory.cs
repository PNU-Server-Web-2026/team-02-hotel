using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelBooking.Api.Tests;

/// <summary>
/// Фабрика тестового сервера для інтеграційних тестів.
/// Запускає API в середовищі "Testing" без Docker і без реальної БД:
/// AppDbContext зареєстрований, але поки що не використовується.
/// Коли з'являться тести, які працюють із БД, тут можна підмінити
/// реєстрацію DbContext (наприклад, на Testcontainers або SQLite).
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}
