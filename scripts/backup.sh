#!/bin/bash

CONTAINER_NAME="polytech_db"
BACKUP_DIR="/home/deploy/backups/polytech_db"
DATE=$(date +"%Y-%m-%d_%H-%M-%S")
ENV_FILE="/home/deploy/Polytech-League/.env"
if [ -f "$ENV_FILE" ]; then
    export $(grep -v '^#' "$ENV_FILE" | xargs)
fi

DB_USER=${DB_USER:-"postgres"}
DB_NAME=${DB_NAME:-"polytech_league"}
RETENTION_DAYS=7

# Налаштування Cloudflare R2 (S3)
UPLOAD_TO_R2=true
R2_BUCKET_NAME=${R2_BUCKET_NAME:-"polytech-db-backups"}
R2_ENDPOINT_URL=${R2_ENDPOINT_URL:-""} # Тепер береться з .env

# Створення папки для бекапів, якщо вона не існує
mkdir -p "$BACKUP_DIR"

# Формування імені файлу бекапу
BACKUP_FILE="$BACKUP_DIR/db_backup_$DATE.sql.gz"

echo "Початок створення бекапу: $BACKUP_FILE"

# Виконання pg_dump всередині контейнера і стиснення gzip
docker exec -t $CONTAINER_NAME pg_dump -U $DB_USER -d $DB_NAME -F p | gzip > "$BACKUP_FILE"

# Перевірка результату виконання
if [ $? -eq 0 ]; then
    echo "Бекап успішно створено: $BACKUP_FILE"
    
    # Завантаження в Cloudflare R2
    if [ "$UPLOAD_TO_R2" = true ]; then
        echo "Завантаження бекапу в Cloudflare R2..."
        # Використовуємо awscli з кастомним endpoint-url
        aws s3 cp "$BACKUP_FILE" "s3://$R2_BUCKET_NAME/" --endpoint-url "$R2_ENDPOINT_URL"
        
        if [ $? -eq 0 ]; then
            echo "Бекап успішно завантажено в Cloudflare R2."
        else
            echo "Помилка при завантаженні в Cloudflare R2!"
        fi
        
    fi
else
    echo "Помилка при створенні бекапу!"
    exit 1
fi

# Видалення старих локальних бекапів (старше RETENTION_DAYS днів)
echo "Видалення локальних бекапів старших за $RETENTION_DAYS днів..."
find "$BACKUP_DIR" -type f -name "db_backup_*.sql.gz" -mtime +$RETENTION_DAYS -exec rm {} \;

echo "Процес бекапу завершено."
