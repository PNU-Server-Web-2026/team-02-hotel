namespace HotelBooking.Api.Tests;

/// <summary>
/// Колекція інтеграційних тестів: усі класи з атрибутом
/// [Collection(IntegrationTestCollection.Name)] отримують один спільний
/// <see cref="ApiFactory"/> (один контейнер PostgreSQL) і виконуються послідовно.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Integration";
}
