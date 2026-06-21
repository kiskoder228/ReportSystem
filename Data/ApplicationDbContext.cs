using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReportSystem.Models;

namespace ReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<ReportStatus> ReportStatuses { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Author)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.AuthorId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.ReviewedBy)
            .WithMany(u => u.ReviewedReports)
            .HasForeignKey(r => r.ReviewedById);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Violator)
            .WithMany(u => u.Violations)
            .HasForeignKey(r => r.ViolatorId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Status)
            .WithMany()
            .HasForeignKey(r => r.StatusId);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Teacher" },
            new Role { Id = 3, Name = "Student" }
        );

        modelBuilder.Entity<ReportStatus>().HasData(
            new ReportStatus { Id = 1, Name = "Ожидает приговора" },
            new ReportStatus { Id = 2, Name = "В разработке" },
            new ReportStatus { Id = 3, Name = "Виновен (Принято)" },
            new ReportStatus { Id = 4, Name = "Оправдан (Отклонено)" }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Списывание", Description = "Списывание на экзамене", SeverityLevel = 2 },
            new Category { Id = 2, Name = "Буллинг", Description = "Травля", SeverityLevel = 3 },
            new Category { Id = 3, Name = "Прогулы", Description = "Пропуск занятий", SeverityLevel = 1 },
            new Category { Id = 4, Name = "Хулиганство", Description = "Нарушение порядка", SeverityLevel = 2 },
            new Category { Id = 5, Name = "Другое", Description = "Иные нарушения", SeverityLevel = 1 }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FullName = "Администратор Системы",
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                RoleId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
