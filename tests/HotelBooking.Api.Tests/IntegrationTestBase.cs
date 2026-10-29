namespace HotelBooking.Api.Tests;

/// <summary>
/// Базовий клас для інтеграційних тестів.
/// Дає спільний <see cref="ApiFactory"/> і HttpClient та очищає БД перед кожним тестом
/// (xUnit створює новий екземпляр тестового класу на кожен тест).
/// Похідний клас обов'язково позначайте [Collection(IntegrationTestCollection.Name)].
/// </summary>
public abstract class IntegrationTestBase(ApiFactory factory) : IAsyncLifetime
{
    protected ApiFactory Factory { get; } = factory;

    protected HttpClient Client { get; } = factory.CreateClient();

    public Task InitializeAsync() => Factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
