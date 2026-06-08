using System.Collections.Generic;
using ReportSystem.Models;

namespace ReportSystem.Data.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
}
