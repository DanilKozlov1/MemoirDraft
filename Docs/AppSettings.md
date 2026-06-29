# Файл конфигурации приложения

**appsettings.json** - файл конфигурации приложения, необходимый для его корректной работы.

Шаблон .Example.json:
```json
{
  "ConnectionStrings": {
    "Default": "Host=YOUR_HOST;Port=YOUR_PORT;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Debug",
        "System": "Debug"
      }
    }
  },
  "Settings": {
    "AppMode" : "Auth"
  },
  "Storage": {
    "Mode": "DatabaseAndFile",
    "NotesPath": "Notes"
  }
}
```

## Блоки конфигурации

### ConnectionStrings
- Блок подключения к базе данных. 
- Обладает одним ключём **Default** - базовая строка подключения к бд.
```json
"Default": "Host=YOUR_HOST;Port=YOUR_PORT;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
```

### Serilog
- Блок конфигурации Serilog`а.
- Подблок **MinimumLevel** - настройка того, логи какого уровня попадают в файл.
- Ключи:
    - **Default** - с какого уровня логи попадают в файл. 
    - **Override** - переопределение уровня для отдельных библиотек.

```json
"Serilog": {
"MinimumLevel": {
    "Default": "Debug",
    "Override": {
    "Microsoft": "Debug",
    "System": "Debug"
    }
}
```

### Settings
- Блок базовых настроек приложения.
- Ключи:
    - **AppMode** - режим запуска приложения.

```json
"Settings": {
    "AppMode" : "Auth"
},
```

### Storage
- Блок настроек хранения заметок
- Ключи:
    - **Mode** - режим работы сохранения заметок.
    - **NotesPath** - название папки, где сохраняются заметки как файл.

```json
"Storage": {
    "Mode": "DatabaseAndFile",
    "NotesPath": "Notes"
}
```