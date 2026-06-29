# Инструкция по запуску базы данных в Docker
Проект использует PostgreSQL 16, запущенную в Docker-контейнере. Для упрощения управления применяется Docker Compose. Конфиденциальные данные (пароль) вынесены в файл .env.

## Требования
- Установленный Docker и Docker Compose (или Docker Desktop с WSL2 на Windows):
    - Для Windows - Docker Desktop в режиме Windows-контейнеров (или WSL2)
    - Для Ubuntu – стандартный Docker Engine + docker-compose-plugin

## Пошаговый запуск базы данных
Все команды выполняются из корневой директории проекта (там, где лежат Dockerfile, docker-compose.yml и .env.example).

- Шаг 1. Создание файла с переменными окружения
Скопируйте шаблон .env.example в .env:

    - Ubuntu (терминал):
        ```bash
        cp .env.example .env
        ```

    - Windows (CMD или PowerShell):
        ```cmd 
        copy .env.example .env
        ```

- Шаг 2. Запуск контейнера через Docker Compose
В терминале (из корня проекта) выполните:

    - Ubuntu (возможно, потребуется sudo):
        ```bash
        sudo docker-compose up -d
        ```

    - Windows (Docker Desktop):
        ```cmd
        docker-compose up -d
        ```

    При первом запуске:
    1. Соберётся образ PostgreSQL на основе Dockerfile.
    2. Создастся том pg_memoirdraft_data для хранения данных.
    3. Запустится контейнер с именем memoirdraft-db.
    4. База будет доступна на порту контейнера.

- Шаг 3. Проверка работоспособности
    ```bash
    docker ps | grep delivery-postgres
    ```

Вы должны увидеть работающий контейнер. Также можно подключиться через любой PostgreSQL-клиент (DBeaver, pgAdmin, DataGrip) по адресу localhost:5432, пользователь postgres, пароль из .env.

## Настройка подключения в WPF приложении
В файле appsettings.json проекта строка подключения и параметры из .env:
```json
   {
    "ConnectionStrings": {
        "Default": "Host=YOUR_HOST;Port=YOUR_PORT;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD"
    },
   }
```

При первом запуске WPF-приложения Entity Framework Core автоматически:
1. Применит миграции.
2. Заполнит базу начальными данными.

## Полезные команды для управления
Остановка контейнера (данные сохраняются):
```bash
docker-compose stop
```

Запуск ранее остановленного контейнера:
```bash
docker-compose start
```

Полная остановка и удаление контейнера (том с данными остаётся):
```bash
docker-compose down
```

Полный сброс (удаление контейнера + тома данных):
```bash
docker-compose down -v
```