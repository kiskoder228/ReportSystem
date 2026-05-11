using Microsoft.EntityFrameworkCore;
using ReportSystem.Models;
using ReportSystem.Models.Enums;

namespace ReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    // Таблицы в базе
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=ep-solitary-thunder-aqe0nh0x-pooler.c-8.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_SodbwOi1JPh4;SSL Mode=Require;Trust Server Certificate=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Указываем связи, так как у нас два внешних ключа на таблицу Users
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Author)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.AuthorId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReviewedBy)
            .WithMany(u => u.ReviewedReports)
            .HasForeignKey(r => r.ReviewedById);

        modelBuilder.Entity<Report>()
            .Property(r => r.Status)
            .HasConversion<string>();

        // Простое заполнение категорий, чтобы не вводить вручную
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Списывание", Description = "Списывание на экзамене", SeverityLevel = 2 },
            new Category { Id = 2, Name = "Буллинг", Description = "Травля", SeverityLevel = 3 },
            new Category { Id = 3, Name = "Прогулы", Description = "Пропуск занятий", SeverityLevel = 1 },
            new Category { Id = 4, Name = "Хулиганство", Description = "Нарушение порядка", SeverityLevel = 2 },
            new Category { Id = 5, Name = "Другое", Description = "Иные нарушения", SeverityLevel = 1 }
        );
    }
}
