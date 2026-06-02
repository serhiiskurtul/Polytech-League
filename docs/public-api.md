# Polytech League - Документація Публічного API

Ласкаво просимо до документації публічного API платформи Polytech League! Цей API створено для того, щоб розробники, дослідники та студенти могли вільно отримувати актуальні дані про спортивні події, турніри та матчі без необхідності авторизації.

## Базова URL-адреса
Всі запити повинні виконуватися до базової URL-адреси вашого розгорнутого бекенду.
```text
https://[домен-бекенду]
```

## Авторизація та CORS
- **Авторизація:** Для доступу до публічних ендпоінтів (`/api/public/*`) не потрібні жодні ключі (API Keys) чи токени. Доступ є повністю відкритим.
- **CORS:** Налаштовано політику `AllowAnyOrigin`. Ви можете робити запити безпосередньо з фронтенду вашого додатку (браузера) без помилок CORS.

---

## Ендпоінти (Endpoints)

### 1. Отримати список спортивних подій
Повертає список усіх доступних спортивних подій (турнірів) з базовою інформацією.

- **URL:** `/api/public/events`
- **Метод:** `GET`
- **Query Параметри:**
  - `sport` *(опціонально)*: Фільтрація за видом спорту (наприклад: `football`, `volleyball`, `chess`, `table-tennis`).

**Приклад запиту:**
```http
GET /api/public/events?sport=football
```

**Приклад відповідь (JSON):**
```json
[
  {
    "id": 1,
    "title": "Осінній кубок з футболу 2026",
    "sport": "football",
    "status": "upcoming",
    "date": "15.09.2026",
    "location": "Головний стадіон",
    "description": "Щорічний турнір серед студентів...",
    "level": "CollegeGroup",
    "category": "Men",
    "participants": 0,
    "teams": 8,
    "isTeam": true,
    "mainPhotoUrl": "/images/tournaments/football-cup.jpg"
  }
]
```

---

### 2. Отримати деталі конкретної події
Повертає вичерпну інформацію про конкретну подію за її ідентифікатором (ID).

- **URL:** `/api/public/events/{id}`
- **Метод:** `GET`
- **Параметри шляху:**
  - `id` *(обов'язково)*: Унікальний ідентифікатор події (число).

**Приклад запиту:**
```http
GET /api/public/events/1
```

**Приклад відповідь (JSON):**
```json
{
  "id": 1,
  "title": "Осінній кубок з футболу 2026",
  "sport": "football",
  "status": "upcoming",
  "date": "15.09.2026",
  "location": "Головний стадіон",
  "description": "Детальний опис турніру...",
  "level": "CollegeGroup",
  "category": "Men",
  "participants": 0,
  "teams": 8,
  "prize": "Кубок та медалі",
  "organizer": "Студентська рада",
  "registrationStart": "01.09.2026",
  "registrationEnd": "10.09.2026",
  "isTeam": true,
  "mainPhotoUrl": "/images/tournaments/football-cup.jpg"
}
```

---

## Приклади використання (Examples)

### Приклад на JavaScript (React / Vanilla JS)
Використання вбудованого `fetch` для отримання списку турнірів і виведення їх у консоль.

```javascript
const API_BASE_URL = 'https://[ваш-домен-бекенду]';

async function fetchTournaments(sport = null) {
    try {
        let url = `${API_BASE_URL}/api/public/events`;
        if (sport) {
            url += `?sport=${sport}`;
        }

        const response = await fetch(url);
        
        if (!response.ok) {
            throw new Error(`HTTP Error: ${response.status}`);
        }

        const data = await response.json();
        console.log("Отримані турніри:", data);
        
        // Тут можна додати логіку відображення на UI
        return data;
    } catch (error) {
        console.error("Помилка під час отримання даних:", error);
    }
}

// Виклик функції для отримання футбольних подій
fetchTournaments('football');
```

### Приклад на Python (requests)
Для аналізу даних або написання скриптів на бекенді/скрейпінгу:

```python
import requests

API_BASE_URL = 'https://[ваш-домен-бекенду]'

def get_tournaments(sport=None):
    url = f"{API_BASE_URL}/api/public/events"
    params = {'sport': sport} if sport else {}
    
    try:
        response = requests.get(url, params=params)
        response.raise_for_status() # Перевірка на помилки (404, 500 тощо)
        
        events = response.json()
        print(f"Знайдено {len(events)} подій.")
        
        for event in events:
            print(f"- {event['title']} ({event['sport']}) | Статус: {event['status']}")
            
        return events
    except requests.exceptions.RequestException as e:
        print(f"Помилка підключення до API: {e}")

# Виклик функції для отримання подій з волейболу
get_tournaments('volleyball')
```
