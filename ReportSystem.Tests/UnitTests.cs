using ReportSystem.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;

namespace ReportSystem.Tests;

/// <summary>
/// ЮНИТ-ТЕСТЫ (5 штук)
/// Тестируют чистую бизнес-логику БЕЗ обращения к базе данных.
/// Не требуют подключения к БД — работают с объектами в памяти.
/// </summary>
public class UnitTests
{
    // ===== ТЕСТЫ МОДЕЛИ USER: вычисляемое свойство Rank =====

    /// <summary>
    /// Тест 1: При отрицательном Score пользователь получает ранг "Мученик".
    /// Проверяет нижнюю границу системы рангов.
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIsNegative_ReturnsMuchenik()
    {
        // Arrange — создаём пользователя с отрицательными очками
        var user = new User { Score = -30 };

        // Act — получаем вычисляемое свойство Rank
        var rank = user.Rank;

        // Assert — проверяем ожидаемый результат
        Assert.Equal("Мученик", rank);
    }

    /// <summary>
    /// Тест 2: При Score = 0 (начальное значение) пользователь получает ранг "Подлиза".
    /// Проверяет, что новый пользователь начинает с правильного ранга.
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIsZero_ReturnsPodliza()
    {
        // Arrange
        var user = new User { Score = 0 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Подлиза", rank);
    }

    /// <summary>
    /// Тест 3: При Score > 90 пользователь получает наивысший ранг "Крыса".
    /// Проверяет верхнюю границу системы рангов.
    /// </summary>
    [Fact]
    public void Rank_WhenScoreAbove90_ReturnsKrysa()
    {
        // Arrange
        var user = new User { Score = 100 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Крыса", rank);
    }

    // ===== ТЕСТЫ РАСЧЁТА ДОСТОВЕРНОСТИ (CalculateReliability) =====

    /// <summary>
    /// Тест 4: Базовый расчёт достоверности — обычная жалоба без особенностей.
    /// Формула: базовый балл = 50, без модификаторов.
    /// </summary>
    [Fact]
    public void CalculateReliability_BaseCase_Returns50()
    {
        // Arrange — создаём репозиторий с мок-фабрикой (БД не нужна)
        var mockFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        var repository = new ReportRepository(mockFactory.Object);

        // Act — считаем достоверность обычной жалобы
        int score = repository.CalculateReliability("Нарушение дисциплины", false);

        // Assert
        Assert.Equal(50, score);
    }

    /// <summary>
    /// Тест 5: Анонимная жалоба с ключевым словом "видел".
    /// Формула: 50 (база) - 20 (анонимная) + 35 (ключевое слово) = 65.
    /// </summary>
    [Fact]
    public void CalculateReliability_AnonymousWithKeyword_Returns65()
    {
        // Arrange
        var mockFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        var repository = new ReportRepository(mockFactory.Object);

        // Act — анонимная жалоба, но с ключевым словом "видел"
        int score = repository.CalculateReliability("Я видел нарушение", true);

        // Assert — 50 - 20 + 35 = 65
        Assert.Equal(65, score);
    }
}
