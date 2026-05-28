using System;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Models;

namespace ReportSystem.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<ReportStatus> ReportStatuses { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=ep-solitary-thunder-aqe0nh0x-pooler.c-8.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_SodbwOi1JPh4;SSL Mode=Require;Trust Server Certificate=true");
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
            new ReportStatus { Id = 1, Name = "New" },
            new ReportStatus { Id = 2, Name = "InProgress" },
            new ReportStatus { Id = 3, Name = "Resolved" },
            new ReportStatus { Id = 4, Name = "Rejected" }
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
