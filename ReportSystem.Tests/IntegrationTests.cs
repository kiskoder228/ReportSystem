using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.Tests;

public class IntegrationTests
{
    private static IDbContextFactory<ApplicationDbContext> CreateFactory()
    {
        var dbName = "TestDb_" + Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        using (var seedContext = new ApplicationDbContext(options))
        {
            seedContext.Database.EnsureCreated();
        }

        return new TestDbContextFactory(options);
    }

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

    [Fact]
    public void RegisterUser_SavesUserToDatabase()
    {
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);

        var user = userRepo.RegisterUser("Иванов Иван Иванович", "ivanov", "password123", "г. Москва");

        Assert.NotEqual(0, user.Id);
        Assert.Equal("Иванов Иван Иванович", user.FullName);
        Assert.Equal("ivanov", user.Login);
        Assert.Equal("г. Москва", user.Address);

        Assert.True(userRepo.UserExists("ivanov"));
    }

    [Fact]
    public void ValidateUser_WithCorrectPassword_ReturnsUser()
    {
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        userRepo.RegisterUser("Петров Пётр", "petrov", "secret", "г. Казань");

        var result = userRepo.ValidateUser("petrov", "secret");

        Assert.NotNull(result);
        Assert.Equal("Петров Пётр", result.FullName);
    }

    [Fact]
    public void ValidateUser_WithWrongPassword_ReturnsNull()
    {
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        userRepo.RegisterUser("Сидоров Алексей", "sidorov", "mypass", "г. Сочи");

        var result = userRepo.ValidateUser("sidorov", "wrongpass");
        Assert.Null(result);
    }
    
    [Fact]
    public void AddReport_SavesReportToDatabase()
    {
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);
        var reportRepo = new ReportRepository(factory);

        var author = userRepo.RegisterUser("Автор Жалобы", "author1", "pass", "г. Москва");
        var violator = userRepo.RegisterUser("Нарушитель Закона", "violator1", "pass", "г. Москва");

        var report = new Report
        {
            AuthorId = author.Id,
            ViolatorId = violator.Id,
            CategoryId = 1,
            Description = "Списывал на экзамене по математике",
            IsAnonymous = false,
            ReliabilityScore = 50
        };
        reportRepo.AddReport(report);
        
        Assert.NotEqual(0, report.Id);

        var authorReports = reportRepo.GetReportsByAuthor(author.Id).ToList();
        Assert.Single(authorReports);
        Assert.Equal("Списывал на экзамене по математике", authorReports[0].Description);
    }

    [Fact]
    public void GetTopInformants_ReturnsOnlyStudentsSortedByScore()
    {
        var factory = CreateFactory();
        var userRepo = new UserRepository(factory);

        userRepo.RegisterUser("Студент Один", "student1", "pass", "г. Москва");
        userRepo.RegisterUser("Студент Два", "student2", "pass", "г. Казань");
        userRepo.RegisterUser("Студент Три", "student3", "pass", "г. Сочи");

        using (var db = factory.CreateDbContext())
        {
            var s1 = db.Users.First(u => u.Login == "student1");
            var s2 = db.Users.First(u => u.Login == "student2");
            var s3 = db.Users.First(u => u.Login == "student3");
            s1.Score = 30;
            s2.Score = 80;
            s3.Score = 10;
            db.SaveChanges();
        }

        var top = userRepo.GetTopInformants(2).ToList();

        Assert.Equal(2, top.Count);
        Assert.Equal("Студент Два", top[0].FullName);
        Assert.Equal("Студент Один", top[1].FullName);
    }
}
