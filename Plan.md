# 📚 **ПЛАН КУРСОВОЙ РАБОТЫ**

**Тема:** *«Разработка автоматизированной системы информирования о нарушениях дисциплины в образовательной организации»*

**Технологии:** 🖥️ Avalonia UI | 💻 C# | 🗄️ PostgreSQL

---

## 🎯 **ВВЕДЕНИЕ**
- 🔍 Актуальность темы (проблемы дисциплины в школах/колледжах)
- 🎯 Цель и задачи работы
- 📊 Объект и предмет исследования
- 🛠️ Практическая значимость
- 📐 Методы исследования

---

## 📐 **1. ПРОЕКТИРОВАНИЕ ПРИЛОЖЕНИЯ**

### 📋 **1.1 Постановка требований к системе**
#### ✅ **1.1.1 Функциональные требования**
- 👤 **Авторизация и регистрация**
  - Вход по логину/паролю
  - Разграничение ролей (Ученик / Преподаватель / Администратор)
  - Восстановление пароля
  
- 📨 **Работа с сообщениями**
  - Создание нового сообщения о нарушении
  - Выбор категории нарушения (списывание, буллинг, прогулы и т.д.)
  - Прикрепление доказательств (фото 📸, видео 🎥, документы 📄)
  - Анонимная подача (опционально)
  - Просмотр истории своих сообщений
  - Отслеживание статуса рассмотрения
  
- 🔧 **Администрирование**
  - Просмотр всех сообщений в системе
  - Фильтрация и поиск
  - Изменение статуса (Новое / В работе / Решено / Отклонено)
  - Назначение ответственного
  - Формирование отчётов

#### ⚡ **1.1.2 Нефункциональные требования**
- 🔒 **Безопасность**
  - Хэширование паролей (BCrypt/Argon2)
  - Защита персональных данных 
  - Шифрование конфиденциальной информации
  
- 🚀 **Производительность**
  - Время отклика интерфейса < 200мс
  - Поддержка до 1000 пользователей
  - Оптимизация загрузки файлов
  
- 💻 **Кроссплатформенность**
  - Работа на Windows/Linux/macOS (благодаря Avalonia)
  - Адаптивный интерфейс
  
- 🎨 **Юзабилити**
  - Интуитивно понятный интерфейс
  - Поддержка тёмной/светлой темы
  - Валидация ввода в реальном времени

---

### 🎭 **1.2 Проектирование Use-Case диаграмм**

#### 👥 **1.2.1 Идентификация акторов**
- 🎓 **Ученик (Student)** — основной пользователь, создающий сообщения
- 👨‍ **Преподаватель (Teacher)** — просмотр сообщений по своему классу/предмету
- 🛡️ **Администратор (Admin)** — полная модерация системы
- 💾 **Система (System)** — автоматические уведомления и бэкапы

#### 📝 **1.2.2 Основные сценарии использования**
- **UC-01:** Регистрация нового пользователя
- **UC-02:** Авторизация в системе
- **UC-03:** Создание сообщения о нарушении
- **UC-04:** Прикрепление доказательств
- **UC-05:** Просмотр статуса сообщения
- **UC-06:** Модерация сообщения (для админа)
- **UC-07:** Формирование отчёта
- **UC-08:** Управление справочниками (категории, статусы)

#### 🔗 **1.2.3 Отношения между use-case**
- `<<include>>` — обязательные включения (например, авторизация перед созданием сообщения)
- `<<extend>>` — расширения (добавление фото к сообщению)

---

### 🗄️ **1.3 Проектирование базы данных**

#### 📊 **1.3.1 Концептуальная модель (ER-диаграмма)**
**Сущности:**
- 🔹 **User** (Пользователи)
  - id, login, password_hash, role, full_name, email, created_at
  
- 🔹 **Student** (Ученики)
  - user_id, class_id, grade, enrollment_date
  
- 🔹 **Teacher** (Преподаватели)
  - user_id, subject, office_hours
  
- 🔹 **Report** (Сообщения)
  - id, author_id, category_id, description, status_id, created_at, reviewed_by, resolved_at
  
- 🔹 **Category** (Категории нарушений)
  - id, name, description, severity_level
  
- 🔹 **Evidence** (Доказательства)
  - id, report_id, file_path, file_type, uploaded_at
  
- 🔹 **Status** (Статусы)
  - id, name, color_code

**Связи:**
- User 1:M Report (один пользователь создаёт много сообщений)
- Report 1:M Evidence (одно сообщение имеет много доказательств)
- Category 1:M Report (одна категория у многих сообщений)
- Student M:1 Class (многие ученики в одном классе)

#### 💽 **1.3.2 Физическая модель**
**SQL-скрипты создания таблиц:**
```sql
CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) CHECK (role IN ('Student', 'Teacher', 'Admin')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Reports (
    id SERIAL PRIMARY KEY,
    author_id INT REFERENCES Users(id),
    category_id INT REFERENCES Categories(id),
    description TEXT NOT NULL,
    status_id INT DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Индексы для оптимизации:**
- INDEX idx_reports_author_id
- INDEX idx_reports_created_at
- INDEX idx_reports_status_id

**Триггеры:**
- Автоматическое обновление updated_at
- Логирование изменений статуса

---

### 🏗️ **1.4 Проектирование структуры классов**

#### 📦 **1.4.1 Модель данных (Models)**
```
📁 Models/
├── DbBase.cs (базовый класс с ID)
├── User.cs (наследуется от DbBase)
├── Student.cs (наследуется от User)
├── Teacher.cs (наследуется от User)
├── Report.cs (основная сущность)
├── Evidence.cs (доказательства)
├── Category.cs (справочник)
└── Enums/
    ├── UserRole.cs
    ├── ReportStatus.cs
    └── EvidenceType.cs
```

#### 🔗 **1.4.2 Отношения между классами**
- **Наследование:** Student → User → DbBase
- **Агрегация:** Report ◇— User (автор)
- **Композиция:** Report ◆— Evidence (доказательства)
- **Реализация:** Moderator —|> IReviewable

#### 🎯 **1.4.3 Сервисный слой (Services)**
```
📁 Services/
├── AuthService.cs (аутентификация)
├── ReportService.cs (работа с сообщениями)
├── FileService.cs (загрузка/хранение файлов)
├── NotificationService.cs (уведомления)
└── IUnitOfWork.cs (паттерн Unit of Work)
```

#### 🗃️ **1.4.4 Слой доступа к данным (Repositories)**
```
📁 Repositories/
├── IRepository<T>.cs (базовый интерфейс)
├── UserRepository.cs
├── ReportRepository.cs
└── ApplicationDbContext.cs (EF Core Context)
```

---

### 🔄 **1.5 Диаграмма видов деятельности (Activity Diagram)**

#### 🎬 **1.5.1 Основной сценарий: Создание сообщения**
1.  **Начало** → Авторизация
2. 🔐 Проверка прав доступа
3. 📝 Заполнение формы:
   - Выбор категории из выпадающего списка
   - Ввод описания нарушения
   - Указание даты/времени
4. 📎 **Ветвление:** Прикрепить файлы?
   - ✅ Да → Выбор файла → Валидация (размер, тип) → Загрузка
   - ❌ Нет → Переход к следующему шагу
5. 👁️ Предварительный просмотр
6. ✅ Подтверждение отправки
7. 💾 Сохранение в БД
8. 🔔 Отправка уведомления администратору
9. 🏁 **Конец**

#### 🛡️ **1.5.2 Сценарий модерации**
1. 📥 Получение нового сообщения
2. 👀 Просмотр деталей
3. 🔍 Проверка доказательств
4. 💬 Добавление комментария
5. 🎯 Установка статуса
6. 📤 Уведомление автора о результате

---

### 🎨 **1.6 Проектирование интерфейса**

#### 🖼️ **1.6.1 Макеты окон (Wireframes)**

**Окно 1: Авторизация (LoginView.axaml)**
```
┌─────────────────────────────┐
│   🔐 СИСТЕМА МОНИТОРИНГА    │
├─────────────────────────────┤
│  👤 Логин:  [___________]   │
│  🔒 Пароль: [___________]   │
│  ☐ Запомнить меня           │
│  [🚀 Войти] [❓ Помощь]     │
└─────────────────────────────┘
```

**Окно 2: Главная форма ученика (StudentDashboard.axaml)**
```
┌─────────────────────────────────────┐
│ 👋 Привет, Иван!    [🔔] [👤] [🚪] │
├─────────────────────────────────────┤
│ [📝 Новый донос] [📋 Мои заявки]   │
│                                     │
│ 📊 Статистика:                      │
│ ├─ Всего: 5                         │
│ ├─ В работе: 2                      │
│ └─ Решено: 3                        │
└─────────────────────────────────────┘
```

**Окно 3: Форма создания сообщения (CreateReportView.axaml)**
```
┌───────────────────────────────────┐
│ 📝 Новое сообщение         [❌]   │
├───────────────────────────────────┤
│ Категория: [▼ Списывание    ]  │
│                                   │
│ Описание:                         │
│ ┌─────────────────────────────┐  │
│ │                             │  │
│ │                             │  │
│ └─────────────────────────────┘  │
│                                   │
│ 📎 Прикрепить файлы:             │
│ [📁 Выбрать] или перетащите      │
│                                   │
│ ☐ Анонимно                       │
│                                   │
│ [💾 Отправить]  [💾 Черновик]   │
└───────────────────────────────────┘
```

**Окно 4: Панель администратора (AdminPanel.axaml)**
```
┌─────────────────────────────────────┐
│ 🛡️ ПАНЕЛЬ АДМИНИСТРАТОРА     [🚪] │
├─────────────────────────────────────┤
│ 🔍 [Поиск...]  📅 [Фильтр даты]   │
│                                     │
│ ┌─────────────────────────────────┐│
│ │ ID │ Автор  │ Категория │ Статус││
│ ├────┼───────────────────┼───────┤│
│ │ 42 │ Аноним │ Буллинг   │ 🟡    ││
│ │ 41 │ Петров │ Прогулы   │ 🟢    ││
│ └─────────────────────────────────┘│
│                                     │
│ [✏️ Редактировать] [✅ Принять]   │
└─────────────────────────────────────┘
```

#### 🎨 **1.6.2 Стилизация (Themes)**
- **Светлая тема:** Material Design Light
- **Тёмная тема:** Material Design Dark
- **Цветовая схема:**
  - 🔴 Критично: #D32F2F
  - 🟡 В работе: #FFA000
  - 🟢 Решено: #388E3C
  - 🔵 Инфо: #1976D2

---

### 📚 **1.7 Используемые компоненты и библиотеки**

#### 🛠️ **1.7.1 Основные технологии**
| Компонент | Версия | Назначение |
|-----------|--------|------------|
| **.NET** | 8.0 | Платформа разработки |
| **C#** | 12.0 | Язык программирования |
| **Avalonia UI** | 11.x | Кроссплатформенный UI |
| **Entity Framework Core** | 8.0 | ORM для работы с БД |

#### 📦 **1.7.2 NuGet пакеты**
```xml
<PackageReference Include="Avalonia" Version="11.0.0" />
<PackageReference Include="Avalonia.Desktop" Version="11.0.0" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.0" />
```

#### 🧪 **1.7.3 Инструменты тестирования**
- **xUnit/NUnit** — модульное тестирование
- **Moq** — мокирование зависимостей
- **Avalonia.Headless** — UI тестирование

#### 📊 **1.7.4 Вспомогательные библиотеки**
- **Serilog** — логирование
- **FluentValidation** — валидация данных
- **AutoMapper** — маппинг объектов
- **Newtonsoft.Json** — работа с JSON

---

## 💻 **2. РЕАЛИЗАЦИЯ ПРИЛОЖЕНИЯ**

### 📱 **2.1 Описание разработанного приложения**

#### 🚪 **2.1.1 Начальное окно приложения (LoginWindow)**
**Реализация:**
- XAML разметка с использованием Grid/StackPanel
- Валидация полей (DataAnnotations)
- Привязка команд (RelayCommand из CommunityToolkit)
- Анимация ошибок (Shake effect)

**Код:**
```csharp
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new LoginViewModel();
    }
}

public class LoginViewModel : ViewModelBase
{
    [Required(ErrorMessage = "Введите логин")]
    public string Login { get; set; }
    
    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; }
    
    public RelayCommand LoginCommand { get; }
    
    private async Task Login()
    {
        if (await _authService.Authenticate(Login, Password))
        {
            // Переход к главному окну
        }
    }
}
```

#### 🏠 **2.1.2 Главное окно ученика (StudentMainWindow)**
**Функционал:**
- Navigation между страницами (Avalonia Navigation)
- DataGrid с последними сообщениями
- Графики статистики (LiveCharts2)
- Система уведомлений (Toast notifications)

**Структура:**
```
📁 Views/Student/
├── StudentMainWindow.axaml
├── CreateReportView.axaml
├── MyReportsView.axaml
└── ProfileView.axaml
```

#### 📝 **2.1.3 Форма создания сообщения**
**Особенности реализации:**
- Drag & Drop загрузка файлов
- Предпросмотр изображений
- Валидация в реальном времени
- Автосохранение черновика

**Ключевой код:**
```csharp
public class CreateReportViewModel : ViewModelBase
{
    public ObservableCollection<EvidenceViewModel> EvidenceFiles { get; }
    
    public async Task UploadFile(string path)
    {
        var file = new Evidence 
        {
            FilePath = path,
            FileType = Path.GetExtension(path)
        };
        
        await _fileService.SaveAsync(file);
        EvidenceFiles.Add(new EvidenceViewModel(file));
    }
    
    public async Task SubmitReport()
    {
        var report = new Report
        {
            AuthorId = _currentUser.Id,
            CategoryId = SelectedCategory.Id,
            Description = Description,
            CreatedAt = DateTime.Now
        };
        
        await _reportService.CreateAsync(report);
    }
}
```

#### 🛡️ **2.1.4 Панель администратора**
**Функционал:**
- Фильтрация по статусу/дате/категории
- Массовые операции (выбор нескольких)
- Экспорт отчётов (PDF/Excel)
- Управление пользователями

**DataGrid с сортировкой:**
```xml
<DataGrid Items="{Binding Reports}" 
          SortModeForColumnHeader="Automatic">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding Id}"/>
        <DataGridTextColumn Header="Автор" Binding="{Binding Author.FullName}"/>
        <DataGridTemplateColumn Header="Статус">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <Border Classes="{Binding StatusColor}">
                        <TextBlock Text="{Binding Status.Name}"/>
                    </Border>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>
```

---

### 🧪 **2.2 Пользовательские сценарии (Тестирование)**

#### ✅ **2.2.1 Сценарий 1: Успешная отправка анонимного сообщения**
**Предусловия:**
- Пользователь авторизован
- Есть доступ к интернету

**Шаги:**
1. Открыть форму создания сообщения
2. Выбрать категорию "Буллинг"
3. Ввести описание: "Оскорбления в чате класса"
4. Отметить чекбокс "Анонимно"
5. Прикрепить скриншот переписки
6. Нажать "Отправить"

**Ожидаемый результат:**
- ✅ Сообщение сохранено в БД
- ✅ Статус установлен "Новое"
- ✅ Администратор получил уведомление
- ✅ Появилось toast-уведомление "Отправлено успешно"

**Тест:**
```csharp
[Fact]
public async Task SubmitAnonymousReport_ShouldSucceed()
{
    // Arrange
    var viewModel = new CreateReportViewModel();
    viewModel.Category = _categories.Bullying;
    viewModel.Description = "Test description";
    viewModel.IsAnonymous = true;
    
    // Act
    await viewModel.SubmitCommand.ExecuteAsync(null);
    
    // Assert
    _reportService.Received(1).CreateAsync(
        Arg.Is<Report>(r => r.IsAnonymous == true && r.StatusId == 1));
}
```

#### ❌ **2.2.2 Сценарий 2: Валидация обязательных полей**
**Шаги:**
1. Открыть форму создания сообщения
2. Не заполнять поле "Описание"
3. Нажать "Отправить"

**Ожидаемый результат:**
- ❌ Появится ошибка "Описание обязательно"
- ❌ Сообщение НЕ сохранено
- 🔴 Поле подсвечено красным

#### 🔄 **2.2.3 Сценарий 3: Изменение статуса модератором**
**Шаги:**
1. Администратор открывает панель
2. Выбирает сообщение со статусом "Новое"
3. Меняет статус на "В работе"
4. Добавляет комментарий
5. Сохраняет

**Ожидаемый результат:**
- ✅ Статус обновлён в БД
- ✅ Автор получил email-уведомление
- ✅ Зафиксировано время изменения

---

## 🎯 **ЗАКЛЮЧЕНИЕ**

**Основные результаты:**
1. ✅ Разработано кроссплатформенное приложение на Avalonia UI
2. ✅ Реализована система разграничения прав доступа
3. ✅ Внедрена возможность анонимной подачи сообщений
4. ✅ Обеспечена защита персональных данных
5. ✅ Создана документация пользователя

**Достижения:**
- 🎓 Получен опыт работы с MVVM паттерном
- 💡 Освоена работа с Entity Framework Core
-  Реализована кроссплатформенность (Windows/Linux)
- 🔒 Применены лучшие практики безопасности

**Перспективы развития:**
- 📱 Мобильная версия (Avalonia для Android/iOS)
- 🤖 Интеграция с Telegram-ботом
- 📊 Расширенная аналитика (Power BI)
- 🔔 Push-уведомления

---

## 📚 **СПИСОК ИСПОЛЬЗОВАННЫХ ИСТОЧНИКОВ**

1.  Microsoft Docs. "Avalonia UI Documentation" [Электронный ресурс]
2. 📖 Troelsen A. "C# 12 and .NET 8" — Apress, 2023
3. 📖 Freeman A. "Pro ASP.NET Core 8" — Apress, 2024
4. 📖 ГОСТ 34.601-90 "Автоматизированные системы. Стадии создания"
5. 📖 Martin R. "Clean Architecture" — Prentice Hall, 2018
6. 📖 Fowler M. "Patterns of Enterprise Application Architecture" — 2002

---

## 📎 **ПРИЛОЖЕНИЯ**

### **ПРИЛОЖЕНИЕ А** 📊 ER-диаграмма базы данных
*(Полноразмерная схема связей таблиц)*

### **ПРИЛОЖЕНИЕ Б** 🔄 Диаграмма последовательности (Sequence Diagram)
*(Взаимодействие объектов при создании сообщения)*

### **ПРИЛОЖЕНИЕ В** 🎨 Макеты интерфейса
*(Скриншоты всех окон приложения)

### **ПРИЛОЖЕНИЕ Г** 💻 Листинг кода
*(Основные фрагменты кода: AuthService, ReportService, ViewModels)*

### **ПРИЛОЖЕНИЕ Д** 📋 Руководство пользователя
*(Пошаговая инструкция по работе с системой)*
