# Features — вертикальні зрізи (vertical slices)

Кожна фіча (бізнес-можливість) живе у власній папці `Features/<Name>/`
і містить усе, що їй потрібно:

```
Features/
  Hotels/
    HotelsController.cs            # HTTP-ендпоінти
    HotelDtos.cs                   # DTO запитів/відповідей (records)
    IHotelService.cs               # інтерфейс сервісу
    HotelService.cs                # реалізація бізнес-логіки
    Hotel.cs                       # сутність EF Core (якщо фіча її вводить)
    HotelConfiguration.cs          # IEntityTypeConfiguration<Hotel>
    HotelsFeatureExtensions.cs     # services.AddHotelsFeature()
```

Правила:

- **Одна фіча — одна папка.** Не створюйте глобальних папок `Controllers/`, `Services/`, `Dtos/`.
- **Namespace = шлях до папки**, наприклад `HotelBooking.Api.Features.Hotels`.
- **Реєстрація сервісів** — через extension-метод у папці фічі
  (`public static IServiceCollection AddHotelsFeature(this IServiceCollection services)`),
  а в `Program.cs` додається лише один рядок `builder.Services.AddHotelsFeature();`.
  Так менше конфліктів злиття у `Program.cs`.
- **Контролер не містить бізнес-логіки** — лише приймає запит, викликає сервіс і повертає результат.
- **Назовні віддаємо DTO, а не сутності EF Core.**
- Код, який справді спільний для кількох фіч (middleware, базові класи, розширення),
  кладемо в `Common/`.

Приклад найпростішої фічі — `Ping/`.
