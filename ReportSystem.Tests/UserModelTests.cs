using ReportSystem.Models;

namespace ReportSystem.Tests;

/// <summary>
/// Тесты для вычисляемого свойства Rank модели User.
/// Ранги определяются по значению Score:
///   Score < 0    → "Мученик"
///   Score ≤ 20   → "Подлиза"
///   Score ≤ 50   → "Смотрящий"
///   Score ≤ 90   → "Стукач"
///   Score > 90   → "Крыса"
/// </summary>
public class UserModelTests
{
    /// <summary>
    /// Тест 1: Проверяет, что при отрицательном Score пользователь получает ранг "Мученик".
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIsNegative_ReturnsMuchenik()
    {
        // Arrange
        var user = new User { Score = -10 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Мученик", rank);
    }

    /// <summary>
    /// Тест 2: Проверяет, что при Score = 0 пользователь получает ранг "Подлиза" (нижняя граница диапазона 0–20).
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
    /// Тест 3: Проверяет, что при Score = 20 (верхняя граница диапазона) пользователь получает ранг "Подлиза".
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIs20_ReturnsPodliza()
    {
        // Arrange
        var user = new User { Score = 20 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Подлиза", rank);
    }

    /// <summary>
    /// Тест 4: Проверяет, что при Score = 50 пользователь получает ранг "Смотрящий".
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIs50_ReturnsSmotrjaschij()
    {
        // Arrange
        var user = new User { Score = 50 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Смотрящий", rank);
    }

    /// <summary>
    /// Тест 5: Проверяет, что при Score = 100 пользователь получает наивысший ранг "Крыса".
    /// </summary>
    [Fact]
    public void Rank_WhenScoreIs100_ReturnsKrysa()
    {
        // Arrange
        var user = new User { Score = 100 };

        // Act
        var rank = user.Rank;

        // Assert
        Assert.Equal("Крыса", rank);
    }
}
