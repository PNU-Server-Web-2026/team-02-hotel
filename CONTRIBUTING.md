# Правила роботи над проєктом

Цей документ — обов'язкові правила для всіх учасників команди.
Загальні правила курсу — у репозиторії [server-web-common](../../../server-web-common).

## Робочий цикл задачі

```
issue → assign → IN PROGRESS → гілка від dev → коміти → PR у dev → REVIEW
      → рев'ю одногрупника + викладача → захист → merge → DONE
```

1. **Issue.** Кожна зміна починається з issue (шаблон «Задача» або «Баг»).
   Без issue — без PR.
2. **Assign.** Призначте issue на себе і перемістіть картку на дошці в **IN PROGRESS**.
3. **Гілка.** Створіть гілку від актуального `dev`:
   ```bash
   git switch dev
   git pull
   git switch -c feat/42/hotel-search
   ```
4. **Коміти.** Робіть невеликі логічні коміти за [Conventional Commits](#коміти-conventional-commits).
   Пушіть гілку регулярно (хоча б раз на день роботи).
5. **PR у `dev`.** Відкрийте Pull Request у гілку `dev`, заповніть шаблон
   (обов'язково `Closes #<номер issue>`), перемістіть картку в **REVIEW**.
6. **Рев'ю.** Потрібен approve **одногрупника** та **викладача**.
   Виправляйте зауваження новими комітами, відповідайте на коментарі.
7. **Захист.** Ви пояснюєте викладачеві своє рішення: що зроблено, чому саме так,
   як це перевірено. Ви маєте вміти пояснити **кожен рядок** свого PR.
8. **Merge.** Після захисту PR зливається (squash merge), issue закривається автоматично,
   картка переходить у **DONE**.

## Гілки

Формат: `<тип>/<номер-issue>/<short-slug>`, лише **lowercase**, слова через дефіс (**kebab-case**).

| Тип      | Для чого                                 | Приклад                          |
|----------|------------------------------------------|----------------------------------|
| `feat/`  | нова функціональність                    | `feat/42/hotel-search`           |
| `fix/`   | виправлення помилки                      | `fix/57/booking-date-validation` |
| `test/`  | лише тести                               | `test/61/rooms-integration`      |
| `chore/` | інфраструктура, залежності, CI, конфіги  | `chore/12/add-health-checks`     |
| `docs/`  | документація                             | `docs/8/update-readme`           |

Гілки `main` і `dev` захищені: прямий push заборонений, лише через PR.

## Коміти (Conventional Commits)

Формат: `<тип>: <що зроблено>` — коротко, в наказовому способі.

| Тип         | Коли використовувати                              |
|-------------|---------------------------------------------------|
| `feat:`     | нова функціональність                             |
| `fix:`      | виправлення помилки                               |
| `test:`     | додавання/зміна тестів                            |
| `refactor:` | зміна коду без зміни поведінки                    |
| `chore:`    | залежності, конфігурація, CI, дрібна інфраструктура |
| `docs:`     | документація                                      |

Приклади:

```
feat: add GET /api/hotels with paging
fix: return 404 when booking not found
test: add integration tests for hotel search
refactor: extract price calculation into service
```

## Pull Request

- PR відкривається **лише в `dev`**. У `main` зливає тільки викладач (релізи).
- Злиття — **squash merge** (одна задача = один коміт у `dev`).
- Обов'язкові умови для merge:
  - **1 approve одногрупника**;
  - **approve викладача** (він указаний у `CODEOWNERS`);
  - **зелений CI** (перевірка `build-and-test`);
  - **усі розмови (conversations) resolved**.
- Один PR = одна задача. Не змішуйте в PR непов'язані зміни.
- Невеликі PR рев'юються швидше. Якщо задача розростається — розбийте її.

## Ліміти WIP

- Максимум **1 задача в IN PROGRESS** і **1 задача в REVIEW** на одного студента.
- Задача, по якій **7 днів** немає ні коміту, ні PR, повертається в **READY**
  (знімається assign), і її може взяти інший учасник.

## Міграції EF Core

- Міграція входить **у PR тієї задачі, що змінює модель** (сутності чи їх конфігурацію).
- Перед merge зробіть **rebase на актуальний `dev`**. Якщо в `dev` з'явилися нові міграції:
  1. видаліть **свою** міграцію (файли міграції й зміни в `AppDbContextModelSnapshot.cs`
     — найпростіше через `dotnet ef migrations remove` **до** rebase, або вручну після нього);
  2. згенеруйте її заново поверх актуального `dev`:
     ```bash
     git fetch origin
     git rebase origin/dev
     dotnet ef migrations add <НазваМіграції> --project src/HotelBooking.Api
     ```
  3. перевірте, що міграція застосовується: `dotnet ef database update --project src/HotelBooking.Api`.
- Ніколи не редагуйте вручну міграції, які вже злиті в `dev`.
- Інструмент `dotnet-ef` встановлюється локально: `dotnet tool restore`.

## Організація коду

- Код організований **вертикальними зрізами**: кожна фіча — у папці
  `src/HotelBooking.Api/Features/<Name>/` (контролер, DTO, інтерфейс сервісу та реалізація,
  сутність і її конфігурація). Деталі — у [Features/README.md](src/HotelBooking.Api/Features/README.md).
- **Реєстрація сервісів фічі — через extension-метод у папці фічі**, щоб не конфліктувати в `Program.cs`:

  ```csharp
  // Features/Hotels/HotelsFeatureExtensions.cs
  namespace HotelBooking.Api.Features.Hotels;

  public static class HotelsFeatureExtensions
  {
      public static IServiceCollection AddHotelsFeature(this IServiceCollection services)
      {
          services.AddScoped<IHotelService, HotelService>();
          return services;
      }
  }
  ```

  ```csharp
  // Program.cs — лише один рядок на фічу
  builder.Services.AddHotelsFeature();
  ```

- Версії NuGet-пакетів задаються **лише** в `Directory.Packages.props`.
- Збірка налаштована з `TreatWarningsAsErrors` — будь-яке попередження ламає CI.
  Виправляйте попередження, а не вимикайте їх.
- Секрети (паролі, ключі) не комітимо: використовуйте `.env` (для Docker) та `dotnet user-secrets`.

## Тести

- Кожна нова фіча/ендпоінт — з інтеграційними тестами в `tests/HotelBooking.Api.Tests/`
  (структура папок дзеркалить `Features/`).
- Перед пушем запустіть локально: `dotnet build` і `dotnet test`.
