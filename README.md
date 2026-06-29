# MemoirDraft

Десктопное приложение для заметок и списков дел с поддержкой базы данных.

## Описание функций

- Создание заметок двух типов: текстовая и список дел.
- Импортирование заметок.
- Возможность сохранения заметок в базу данных, если они были сохранены только в файлах.

## Технологии

| Слой | Технология |
|---|---|
| UI | WPF (.NET 9), XAML |
| Паттерн | MVVM |
| БД | PostgreSQL + Entity Framework Core |
| DI | Microsoft.Extensions.DependencyInjection |
| Логирование | Serilog |

## Структура проекта

```
MemoirDraft/
├── MemoirDraft.slnx
│
├── Docs/                             # Документация
│
├── Scripts/                          # Скрипты   
│
└── MemoirDraft/
    ├── App.xaml
    ├── App.xaml.cs                       # DI
    ├── appsettings.Example.json          # Шаблон файла appsettings           
    │
    ├── Commands                          # Классы-команды для других классов
    │
    ├── Database/                         # База данных
    │   ├── DTO/                            # DTO для таблиц
    │   ├── Models/                         # Модели таблиц
    │   └── AppDBContext.cs                 # Модель БД
    │
    ├── DTO/                              # Объекты передачи данных
    │   
    ├── Enums/                            # Типы данных
    │  
    ├── Migrations/                       # Миграции
    |
    ├── Repositories/                     # Классы для доступа к данным
    │   └── Interfaces/
    │
    ├── Servises/                         # Бизнес-логика
    │   ├── DatabaseNoteMode/               # Сервисы, отвечающие за работу с бд
    │   ├── FileOnlyNoteMode/               # Сервисы, отвечающие за работу с файлами
    │   └── Interfaces/                 
    │
    ├── Styles/                           # Стили для окон и страниц
    │
    ├── Utils/                            # Классы-инструменты
    │  
    ├── ViewModels/                       # Модели страниц и окон
    │   └── Abstraction/
    │
    └── Views/                            # UI окон и страниц  
```

## Запуск

### Требования

- Visual Studio 2022
- .NET 9 SDK
- PostgreSQL 15+

### Установка

1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/DanilKozlov1/MemoirDraft.git
   cd MemoirDraft
   ```

2. Создать базу данных в PostgreSQL:
   ```sql
   CREATE DATABASE memoirdraft_db;
   ```

3. Настроить строку подключения в `appsettings.json`:
   ```json
   {
    "ConnectionStrings": {
        "Default": "Host=YOUR_HOST;Port=YOUR_PORT;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
    },
   }
   ```

4. Открыть `MemoirDraft.slnx` в Visual Studio и запустить (F5)

## Документация

Вся документация находится в папке Docs. Файлы документации:
```
Docs/
├── App-Modes.md              # Описание режимов работы приложения
├── Architecture.md           # Mermaid-диаграммы 
├── Backup-Restore-Guide.md   # Инструкция для скриптов **backup.sh** и **restore.sh**
└── Docker-Database.md        # Инструкция по запуску бд в контейнере Docker  
```

---