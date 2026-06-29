#!/bin/bash

# Загрузка переменных из .env (файл должен быть в корне)
set -a
source .env
set +a

# Автоматический поиск папки PostgreSQL
for v in 18 17 16 15 14 13; do
    if [ -d "/c/Program Files/PostgreSQL/$v/bin" ]; then
        export PATH="$PATH:/c/Program Files/PostgreSQL/$v/bin"
        break
    fi
done

# Параметры подключения к локальной БД (берутся из .env)
DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-5432}
DB_USER=${DB_USER:-postgres}
DB_PASSWORD=${DB_PASSWORD:-admin}
DB_NAME=${DB_NAME:-memoirdraft_db}

# Папки
BACKUP_ROOT="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_DIR="$BACKUP_ROOT/backup_$TIMESTAMP"
mkdir -p "$BACKUP_DIR"

# Логирование
LOG_FILE="$BACKUP_DIR/backup.log"
exec > >(tee -a "$LOG_FILE") 2>&1
echo "=== Резервное копирование начато $(date) ==="

# 1. Дамп базы данных PostgreSQL (локально)
echo "Создание дампа базы данных..."
export PGPASSWORD="$DB_PASSWORD"
pg_dump -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -F p -f "$BACKUP_DIR/db_dump_$TIMESTAMP.sql"
unset PGPASSWORD

if [ -f "$BACKUP_DIR/db_dump_$TIMESTAMP.sql" ]; then
    echo "Дамп БД успешно создан: $BACKUP_DIR/db_dump_$TIMESTAMP.sql"
else
    echo "ОШИБКА: не удалось создать дамп БД" >&2
    exit 1
fi

# 2. Резервное копирование медиа-файлов (папка uploads)
UPLOADS_DIR="./uploads"
if [ -d "$UPLOADS_DIR" ]; then
    echo "Архивация медиа-файлов..."
    tar -czf "$BACKUP_DIR/uploads_backup_$TIMESTAMP.tar.gz" -C . "$UPLOADS_DIR"
    echo "Архив медиа создан: $BACKUP_DIR/uploads_backup_$TIMESTAMP.tar.gz"
else
    echo "Внимание: папка $UPLOADS_DIR не найдена. Медиа-бэкап пропущен."
fi

echo "=== Резервное копирование завершено $(date) ==="
echo "Бэкап сохранён в: $BACKUP_DIR"