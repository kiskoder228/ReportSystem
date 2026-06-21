using Moq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;

namespace ReportSystem.Tests;

/// <summary>
/// Тесты для метода CalculateReliability из ReportRepository.
/// Формула расчёта достоверности:
///   Базовый балл = 50
///   Анонимная жалоба: -20
///   Описание > 50 символов: +15
///   Ключевые слова ("точно", "видел", "свидетель"): +35
///   Итог зажимается в диапазон [0, 100]
/// </summary>
public class ReliabilityCalculationTests
{
    private readonly ReportRepository _repository;

    public ReliabilityCalculationTests()
    {
        // CalculateReliability не обращается к БД, поэтому достаточно мок-фабрики
        var mockFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        _repository = new ReportRepository(mockFactory.Object);
    }

    /// <summary>
    /// Тест 6: Базовый расчёт — обычная (не анонимная) жалоба с коротким описанием.
    /// Ожидаемый результат: 50 (только базовый балл).
    /// </summary>
    [Fact]
    public void CalculateReliability_BaseCase_Returns50()
    {
        // Arrange: не анонимная, короткое описание без ключевых слов
        string description = "Нарушение дисциплины";
        bool isAnonymous = false;

        // Act
        int score = _repository.CalculateReliability(description, isAnonymous);

        // Assert
        Assert.Equal(50, score);
    }

    /// <summary>
    /// Тест 7: Анонимная жалоба — должен вычесть 20 из базового балла.
    /// Ожидаемый результат: 30 (50 - 20).
    /// </summary>
    [Fact]
    public void CalculateReliability_Anonymous_Returns30()
    {
        // Arrange
        string description = "Нарушение";
        bool isAnonymous = true;

        // Act
        int score = _repository.CalculateReliability(description, isAnonymous);

        // Assert
        Assert.Equal(30, score);
    }

    /// <summary>
    /// Тест 8: Длинное описание (> 50 символов) без ключевых слов и не анонимная.
    /// Ожидаемый результат: 65 (50 + 15).
    /// </summary>
    [Fact]
    public void CalculateReliability_LongDescription_Returns65()
    {
        // Arrange: описание > 50 символов
        string description = "Студент Иванов постоянно нарушает дисциплину на уроках математики и физики в аудитории";
        bool isAnonymous = false;

        // Act
        int score = _repository.CalculateReliability(description, isAnonymous);

        // Assert
        Assert.Equal(65, score);
    }

    /// <summary>
    /// Тест 9: Описание содержит ключевое слово "видел" — добавляет +35.
    /// Короткое описание, не анонимная.
    /// Ожидаемый результат: 85 (50 + 35).
    /// </summary>
    [Fact]
    public void CalculateReliability_WithKeyword_Returns85()
    {
        // Arrange: короткое описание с ключевым словом
        string description = "Я видел как он списывал";
        bool isAnonymous = false;

        // Act
        int score = _repository.CalculateReliability(description, isAnonymous);

        // Assert
        Assert.Equal(85, score);
    }

    /// <summary>
    /// Тест 10: Максимальная комбинация — длинное описание + ключевое слово + не анонимная.
    /// Расчёт: 50 + 15 (длинное) + 35 (ключевое слово) = 100.
    /// Результат зажимается сверху до 100.
    /// </summary>
    [Fact]
    public void CalculateReliability_MaxCombination_Returns100()
    {
        // Arrange: длинное описание (>50 символов) + ключевое слово "свидетель"
        string description = "Я являюсь свидетель того как студент Иванов постоянно списывал на экзаменах в течение семестра";
        bool isAnonymous = false;

        // Act
        int score = _repository.CalculateReliability(description, isAnonymous);

        // Assert
        Assert.Equal(100, score);
    }
}
