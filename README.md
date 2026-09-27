# Hotel Booking API

Командний навчальний проєкт з дисципліни **«Серверне вебпрограмування»**.

> Перед початком роботи обов'язково прочитайте [CONTRIBUTING.md](CONTRIBUTING.md)
> та загальні правила курсу в репозиторії
> [server-web-common](../../../server-web-common).

## Стек

- **ASP.NET Core 10** (Web API, контролери)
- **Entity Framework Core 10** + **PostgreSQL** (Npgsql)
- **SignalR** (real-time, з'явиться в задачах)
- **xUnit** + `WebApplicationFactory` (інтеграційні тести)
- **Docker** / Docker Compose (локальна БД)
- **GitHub Actions** (CI: build + test на кожен PR)

## Що потрібно встановити

- [.NET SDK 10](https://dotnet.microsoft.com/download/dotnet/10.0) (`dotnet --version` → `10.0.x`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (або Docker Engine + Compose v2)
- IDE: Visual Studio 2022/2026, JetBrains Rider або VS Code з C# Dev Kit
- Git

## Швидкий старт

```bash
# 1. (Необов'язково) власні налаштування БД
cp .env.example .env

# 2. Запустити PostgreSQL у Docker
docker compose up -d db

# 3. Відновити локальні інструменти (dotnet-ef)
dotnet tool restore

# 4. Запустити API
dotnet run --project src/HotelBooking.Api

# 5. Запустити тести
dotnet test
```

Після запуску перевірте:

- http://localhost:5080/api/ping → `{"message":"pong","utc":"..."}`
- http://localhost:5080/openapi/v1.json → OpenAPI-документ (лише в середовищі Development)

### Міграції EF Core

```bash
# створити міграцію
dotnet ef migrations add <НазваМіграції> --project src/HotelBooking.Api

# застосувати міграції до БД
dotnet ef database update --project src/HotelBooking.Api
```

Правила роботи з міграціями в команді — у [CONTRIBUTING.md](CONTRIBUTING.md#міграції-ef-core).

### Рядок підключення

За замовчуванням (`appsettings.json`):

```
Host=localhost;Port=5432;Database=hotel_booking;Username=postgres;Password=postgres
```

Якщо у вас інші параметри — не змінюйте `appsettings.json`, а використайте user-secrets:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Host=...;..." --project src/HotelBooking.Api
```

## Структура рішення

```
.
├── HotelBooking.sln
├── Directory.Build.props          # спільні налаштування збірки (net10.0, nullable, warnings as errors)
├── Directory.Packages.props       # версії всіх NuGet-пакетів (Central Package Management)
├── .config/dotnet-tools.json      # локальні інструменти (dotnet-ef)
├── docker-compose.yml             # PostgreSQL для локальної розробки
├── src/
│   └── HotelBooking.Api/
│       ├── Program.cs             # точка входу, реєстрація сервісів і конвеєр HTTP
│       ├── Data/                  # AppDbContext, міграції
│       ├── Features/              # вертикальні зрізи: одна фіча = одна папка
│       │   └── Ping/              # приклад: GET /api/ping
│       └── Common/                # спільний код для кількох фіч
├── tests/
│   └── HotelBooking.Api.Tests/     # інтеграційні тести (xUnit + WebApplicationFactory)
└── .github/                       # CI, шаблони PR та issue, CODEOWNERS
```

Детальніше про організацію коду фіч — у [src/HotelBooking.Api/Features/README.md](src/HotelBooking.Api/Features/README.md).

## Як працюємо

Коротко: **issue → гілка від `dev` → PR у `dev` → рев'ю → захист → merge**.
Повні правила (іменування гілок, коміти, рев'ю, ліміти WIP, міграції) — у [CONTRIBUTING.md](CONTRIBUTING.md).
