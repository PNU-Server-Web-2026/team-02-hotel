using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Api.Data;

/// <summary>
/// Контекст бази даних застосунку.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Сутності (DbSet<T>) додаються в задачах, наприклад:
    // public DbSet<Hotel> Hotels => Set<Hotel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Підхоплює всі класи IEntityTypeConfiguration<T> з цієї збірки.
        // Конфігурацію сутності тримайте поруч із фічею: Features/<Name>/<Entity>Configuration.cs
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
