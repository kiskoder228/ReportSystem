using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class MyReportsViewModel : ObservableObject
{
    public ObservableCollection<Report> Reports { get; } = new();

    public MyReportsViewModel(User user)
    {
        LoadReports(user);
    }

    private void LoadReports(User user)
    {
        try
        {
            using (var db = new ApplicationDbContext())
            {
                // Получаем список жалоб пользователя
                var list = db.Reports.Where(r => r.AuthorId == user.Id).ToList();
                
                foreach (var report in list)
                {
                    // Подгружаем категорию вручную для каждой жалобы
                    report.Category = db.Categories.FirstOrDefault(c => c.Id == report.CategoryId);
                    Reports.Add(report);
                }
            }
        }
        catch (System.Exception)
        {
            // ошибка
        }
    }
}
