# Polytech League - Backend

Ця частина репозиторію містить серверну частину (API) платформи Polytech League, побудовану на базі сучасного стеку технологій від Microsoft.

## 🛠️ Технологічний стек

* **Мова:** C#
* **Фреймворк:** .NET 9, ASP.NET Core Web API
* **База даних:** PostgreSQL
* **ORM:** Entity Framework Core
* **Кешування:** Redis
* **Real-time зв'язок:** SignalR
* **Архітектура:** Clean Architecture (Чиста архітектура)

## 📁 Структура проєкту (Clean Architecture)

Проєкт розділений на декілька шарів для забезпечення незалежності компонентів та легкості тестування:

* `PolytechLeague.Domain`: Основні сутності, моделі предметної області (наприклад, гравець, турнір, матч) та інтерфейси.
* `PolytechLeague.Application`: Бізнес-логіка, сервіси, DTO (Data Transfer Objects), обробники команд та запитів.
* `PolytechLeague.Infrastructure`: Реалізація зовнішніх сервісів, робота з фоновими задачами, генерація токенів тощо.
* `PolytechLeague.Persistence`: Налаштування бази даних, міграції Entity Framework.
* `PolytechLeague.API`: Точка входу в додаток, контролери (Controllers), конфігурація Swagger, налаштування DI (Dependency Injection).
* `PolytechLeague.Tests`: Модульні та інтеграційні тести.

## 🚀 Запуск локально

### Передумови
* Встановлений [.NET 9 SDK](https://dotnet.microsoft.com/download)
* Запущений сервер PostgreSQL
* Запущений сервер Redis

### Кроки для запуску:

1. Перейдіть до папки backend:
   ```bash
   cd backend/src/PolytechLeague.API
   ```

2. Налаштуйте рядок підключення до БД (Connection String) у файлі `appsettings.json` або `appsettings.Development.json`.

3. Застосуйте міграції бази даних:
   ```bash
   dotnet ef database update --project ../PolytechLeague.Persistence --startup-project .
   ```

4. Запустіть проєкт:
   ```bash
   dotnet run
   ```

API буде доступне за адресою `https://localhost:5000` або `http://localhost:5000` (також доступний Swagger UI для тестування ендпоінтів).

### Запуск через Docker
Ви можете запустити весь бекенд разом із базою даних та Redis за допомогою Docker Compose з кореневої папки проєкту:
```bash
docker-compose up -d api db redis
```
