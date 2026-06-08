using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Models;

namespace ReportSystem.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public CategoryRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public IEnumerable<Category> GetAll()
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Categories.ToList();
    }
}
