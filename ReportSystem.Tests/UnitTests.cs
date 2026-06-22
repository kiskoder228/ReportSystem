using ReportSystem.Models;
using Moq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;

namespace ReportSystem.Tests;

public class UnitTests
{
    [Fact]
    public void Rank_WhenScoreIsNegative_ReturnsMuchenik()
    {
        var user = new User { Score = -30 };
        var rank = user.Rank;
        Assert.Equal("Мученик", rank);
    }

    [Fact]
    public void Rank_WhenScoreIsZero_ReturnsPodliza()
    {
        var user = new User { Score = 0 };

        var rank = user.Rank;

        Assert.Equal("Подлиза", rank);
    }

    [Fact]
    public void Rank_WhenScoreAbove90_ReturnsKrysa()
    {
        var user = new User { Score = 100 };

        var rank = user.Rank;

        Assert.Equal("Крыса", rank);
    }
    
    [Fact]
    public void CalculateReliability_BaseCase_Returns50()
    {
        var mockFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        var repository = new ReportRepository(mockFactory.Object);

        int score = repository.CalculateReliability("Нарушение дисциплины", false);

        Assert.Equal(50, score);
    }

    [Fact]
    public void CalculateReliability_AnonymousWithKeyword_Returns65()
    {
        var mockFactory = new Mock<IDbContextFactory<ApplicationDbContext>>();
        var repository = new ReportRepository(mockFactory.Object);

        int score = repository.CalculateReliability("Я видел нарушение", true);
 
        Assert.Equal(65, score);
    }
}
