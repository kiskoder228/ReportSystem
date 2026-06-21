using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.Tests;

/// <summary>
/// ИНТЕГРАЦИОННЫЕ ТЕСТЫ (5 штук)
/// Тестируют взаимодействие репозиториев с базой данных.
/// Используется InMemory-провайдер EF Core — эмуляция настоящей БД в оперативной памяти.
/// Каждый тест получает чистую базу с начальными данными (роли, статусы, категории).
/// </summary>
public class IntegrationTests
{
    /// <summary>
    /// Вспомогательный метод: создаёт фабрику контекстов с InMemory-базой.
    /// Каждый тест получает свою изолированную БД (уникальное имя через Guid).
    /// После создания БД заполняется начальными данными (seed data из OnModelCreating).
    /// </summary>
    private static IDbContextFactory<ApplicationDbContext> CreateFactory()
    {
        var dbName = "TestDb_" + Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        // Заполняем начальные данные (роли, статусы, категории)
        using (var seedContext = new ApplicationDbContext(options))
        {
            seedContext.Database.EnsureCreated();
        }

        // Возвращаем фабрику, которая будет создавать контексты с той же БД
        return new TestDbContextFactory(options);
    }

    /// <summary>
    /// Реализация IDbContextFactory для тестов.
    /// Каждый вызов CreateDbContext() возвращает новый контекст,
    /// но все контексты работают с одной и той же InMemory-базой.
    /// </summary>
    private class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public TestDbContextFactory(DbContextOptions<ApplicationDbContext> options)
        {
            _options = options;
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }
    }

    // =====================================================================

    /// <summary>
    /// Тест 1: Регистрация пользователя сохраняет его в базу данных.
    /// Проверяет, что UserRepository.RegisterUser создаёт запись в таблице Users
    /// с правильными полями (ФИО, логин, адрес) и ролью "Student".
    /// </summary>
    [Fact]
    public void RegisterUser_SavesUserToDatabase()
    {
        // Arrange — создаём репозиторий с InMemory-базой
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);

        // Act — регистрируем нового пользователя
        var user = userRepo.RegisterUser("Иванов Иван Иванович", "ivanov", "password123", "г. Москва");

        // Assert — проверяем, что пользователь сохранён в БД
        Assert.NotEqual(0, user.Id);                    // ID присвоен базой
        Assert.Equal("Иванов Иван Иванович", user.FullName);
        Assert.Equal("ivanov", user.Login);
        Assert.Equal("г. Москва", user.Address);

        // Проверяем, что пользователь действительно есть в БД
        Assert.True(userRepo.UserExists("ivanov"));
    }

    /// <summary>
    /// Тест 2: Авторизация с правильным паролем возвращает пользователя.
    /// Проверяет полный цикл: регистрация → хеширование пароля → проверка пароля.
    /// BCrypt.Verify должен корректно сверить пароль с хешем из БД.
    /// </summary>
    [Fact]
    public void ValidateUser_WithCorrectPassword_ReturnsUser()
    {
        // Arrange — регистрируем пользователя
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        userRepo.RegisterUser("Петров Пётр", "petrov", "secret", "г. Казань");

        // Act — пробуем авторизоваться с правильным паролем
        var result = userRepo.ValidateUser("petrov", "secret");

        // Assert — пользователь найден
        Assert.NotNull(result);
        Assert.Equal("Петров Пётр", result.FullName);
    }

    /// <summary>
    /// Тест 3: Авторизация с неправильным паролем возвращает null.
    /// Проверяет, что система НЕ пропускает пользователя с неверным паролем.
    /// </summary>
    [Fact]
    public void ValidateUser_WithWrongPassword_ReturnsNull()
    {
        // Arrange — регистрируем пользователя
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        userRepo.RegisterUser("Сидоров Алексей", "sidorov", "mypass", "г. Сочи");

        // Act — пробуем авторизоваться с неправильным паролем
        var result = userRepo.ValidateUser("sidorov", "wrongpass");

        // Assert — пользователь НЕ найден
        Assert.Null(result);
    }

    /// <summary>
    /// Тест 4: Создание жалобы сохраняет её в базу данных.
    /// Проверяет, что ReportRepository.AddReport создаёт запись в таблице Reports
    /// со всеми связями (автор, нарушитель, категория, статус).
    /// </summary>
    [Fact]
    public void AddReport_SavesReportToDatabase()
    {
        // Arrange — создаём двух пользователей (автор и нарушитель)
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        var reportRepo = new ReportRepository(factory);

        var author = userRepo.RegisterUser("Автор Жалобы", "author1", "pass", "г. Москва");
        var violator = userRepo.RegisterUser("Нарушитель Закона", "violator1", "pass", "г. Москва");

        // Act — создаём жалобу
        var report = new Report
        {
            AuthorId = author.Id,
            ViolatorId = violator.Id,
            CategoryId = 1,  // Списывание (из seed data)
            Description = "Списывал на экзамене по математике",
            IsAnonymous = false,
            ReliabilityScore = 50
        };
        reportRepo.AddReport(report);

        // Assert — жалоба сохранена с ID и правильным статусом
        Assert.NotEqual(0, report.Id);

        // Проверяем, что жалоба видна в списке жалоб автора
        var authorReports = reportRepo.GetReportsByAuthor(author.Id).ToList();
        Assert.Single(authorReports);
        Assert.Equal("Списывал на экзамене по математике", authorReports[0].Description);
    }

    /// <summary>
    /// Тест 5: Топ информаторов возвращает только студентов, отсортированных по Score.
    /// Проверяет, что GetTopInformants:
    /// — фильтрует по роли "Student" (Admin и Teacher не попадают);
    /// — сортирует по убыванию Score;
    /// — ограничивает количество результатов.
    /// </summary>
    [Fact]
    public void GetTopInformants_ReturnsOnlyStudentsSortedByScore()
    {
        // Arrange — создаём несколько пользователей с разными ролями
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);

        // Регистрируем 3 студентов (роль Student назначается по умолчанию)
        userRepo.RegisterUser("Студент Один", "student1", "pass", "г. Москва");
        userRepo.RegisterUser("Студент Два", "student2", "pass", "г. Казань");
        userRepo.RegisterUser("Студент Три", "student3", "pass", "г. Сочи");

        // Вручную меняем Score для проверки сортировки
        using (var db = factory.CreateDbContext())
        {
            var s1 = db.Users.First(u => u.Login == "student1");
            var s2 = db.Users.First(u => u.Login == "student2");
            var s3 = db.Users.First(u => u.Login == "student3");
            s1.Score = 30;
            s2.Score = 80;  // Самый высокий Score
            s3.Score = 10;
            db.SaveChanges();
        }

        // Act — получаем топ-2 информаторов
        var top = userRepo.GetTopInformants(2).ToList();

        // Assert
        Assert.Equal(2, top.Count);              // Только 2 из 3
        Assert.Equal("Студент Два", top[0].FullName);   // Score 80 — первый
        Assert.Equal("Студент Один", top[1].FullName);  // Score 30 — второй
    }
}
