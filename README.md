# 📋 ReportSystem — Система информирования о нарушениях дисциплины

Кроссплатформенное десктопное приложение для автоматизации процесса подачи, обработки и хранения информации о дисциплинарных нарушениях в образовательной организации.

Разработано в рамках курсовой работы.

---

## 🎯 Назначение

Приложение позволяет ученикам анонимно или открыто сообщать о нарушениях дисциплины (списывание, буллинг, прогулы и др.), а преподавателям и администраторам — оперативно обрабатывать поступившие заявки. Система разграничивает права доступа по ролям и обеспечивает безопасное хранение данных.

---

## 🛠 Стек технологий

| Компонент              | Технология                                    |
|------------------------|-----------------------------------------------|
| Платформа              | .NET 8.0                                      |
| Язык                   | C# 12                                         |
| UI-фреймворк           | Avalonia UI 11.3 (XAML)                       |
| Архитектурный паттерн   | MVVM (CommunityToolkit.Mvvm)                  |
| База данных            | PostgreSQL (Neon — облачный хостинг)           |
| ORM                    | Entity Framework Core 8 (Npgsql)              |
| Хэширование паролей    | BCrypt.Net-Next                               |

---

## ✨ Функциональные возможности

### 🔐 Авторизация и регистрация
- Регистрация новых пользователей с ролью «Ученик»
- Вход в систему по логину и паролю
- Хэширование паролей алгоритмом BCrypt
- Переключение видимости пароля в форме

### 👨‍🎓 Роль: Ученик (Student)
- **Дашборд** — общая статистика по нарушениям
- **Создание заявки** — подача сообщения о нарушении с выбором категории, описанием и возможностью анонимной подачи
- **Мои заявки** — просмотр собственных поданных заявок и их статусов

### 👨‍🏫 Роль: Преподаватель (Teacher)
- **Дашборд** — общая статистика
- **Просмотр заявок** — доступ ко всем поданным заявкам с возможностью фильтрации

### 👨‍💼 Роль: Администратор (Admin)
- **Дашборд** — общая статистика по системе
- **Управление заявками** — просмотр, поиск, фильтрация по статусу, изменение статуса заявки (New → InProgress → Resolved / Rejected)
- **Управление пользователями** — просмотр всех пользователей, изменение ролей, удаление учётных записей

### 📊 Статусы заявок
| Статус       | Описание                    |
|--------------|-----------------------------|
| `New`        | Новая заявка                |
| `InProgress` | В процессе рассмотрения     |
| `Resolved`   | Рассмотрена и решена        |
| `Rejected`   | Отклонена                   |

---

## 🗄 Структура базы данных

```
Users (Пользователи)
├── Id, FullName, Login, PasswordHash, Role, Email, CreatedAt

Categories (Категории нарушений)
├── Id, Name, Description, SeverityLevel

Reports (Заявки о нарушениях)
├── Id, AuthorId → Users, CategoryId → Categories
├── Description, Status, IsAnonymous, CreatedAt
├── ReviewedById → Users, ResolvedAt, ModeratorComment

Evidences (Доказательства)
├── Id, ReportId → Reports, FilePath, FileType, UploadedAt
```

Предустановленные категории: Списывание, Буллинг, Прогулы, Хулиганство, Другое.

---

## 📁 Структура проекта

```
ReportSystem/
├── Models/                  # Модели данных (сущности БД)
│   ├── User.cs
│   ├── Report.cs
│   ├── Category.cs
│   ├── Evidence.cs
│   └── Enums/
│       └── ReportStatus.cs
├── Data/                    # Контекст базы данных (EF Core)
│   └── ApplicationDbContext.cs
├── ViewModels/              # Логика представления (MVVM)
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── MainWindowViewModel.cs
│   ├── DashboardPageViewModel.cs
│   ├── CreateReportViewModel.cs
│   ├── MyReportsViewModel.cs
│   ├── AdminReportsViewModel.cs
│   └── AdminUsersViewModel.cs
├── Views/                   # Пользовательский интерфейс (AXAML)
│   ├── LoginWindow.axaml
│   ├── RegisterWindow.axaml
│   ├── MainWindow.axaml
│   ├── DashboardPage.axaml
│   ├── CreateReportPage.axaml
│   ├── MyReportsPage.axaml
│   ├── AdminReportsPage.axaml
│   └── AdminUsersPage.axaml
├── Migrations/              # Миграции EF Core
├── App.axaml                # Конфигурация приложения и стили
├── Program.cs               # Точка входа
└── ReportSystem.csproj      # Файл проекта
```

---

## 🚀 Установка и запуск

### Требования
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- IDE: Visual Studio 2022 / JetBrains Rider / VS Code

### Шаги запуска

1. **Клонируйте репозиторий:**
   ```bash
   git clone https://github.com/ВАШ_ЛОГИН/ReportSystem.git
   cd ReportSystem
   ```

2. **Восстановите зависимости:**
   ```bash
   dotnet restore
   ```

3. **Настройте подключение к БД** (файл `Data/ApplicationDbContext.cs`):
   ```csharp
   optionsBuilder.UseNpgsql("Host=ВАШ_ХОСТ;Database=ИМЯ_БД;Username=ЛОГИН;Password=ПАРОЛЬ;SSL Mode=Require;Trust Server Certificate=true");
   ```

4. **Примените миграции:**
   ```bash
   dotnet ef database update
   ```

5. **Запустите приложение:**
   ```bash
   dotnet run
   ```

---

## 📝 Использование

1. При первом запуске откроется **окно входа**.
2. Нажмите **«Зарегистрироваться»**, чтобы создать учётную запись (роль — Ученик).
3. Войдите в систему с вашим логином и паролем.
4. Для создания администратора — используйте панель управления пользователями от имени существующего администратора, либо измените роль через БД напрямую.

---

## 👤 Автор

Семагаев Данил 1125
Потапов Данил 1125
