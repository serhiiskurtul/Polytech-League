import React, { useState } from 'react';
import styles from './ApiDocsPage.module.css';
import toast from 'react-hot-toast';

export const ApiDocsPage = () => {
  const [copied, setCopied] = useState<string | null>(null);

  const handleCopy = (text: string, id: string) => {
    navigator.clipboard.writeText(text);
    setCopied(id);
    toast.success('Скопійовано!');
    setTimeout(() => setCopied(null), 2000);
  };

  const API_BASE_URL = window.location.origin.includes('localhost') 
    ? 'https://localhost:7154' 
    : 'https://polytechleague.azurewebsites.net'; // Fallback or update with actual

  const jsExample = `const API_BASE_URL = '${API_BASE_URL}';

async function fetchTournaments(sport = null) {
  try {
    let url = \`\${API_BASE_URL}/api/public/events\`;
    if (sport) url += \`?sport=\${sport}\`;

    const response = await fetch(url);
    if (!response.ok) throw new Error(\`HTTP Error: \${response.status}\`);

    const data = await response.json();
    console.log("Отримані турніри:", data);
    return data;
  } catch (error) {
    console.error("Помилка під час отримання даних:", error);
  }
}

fetchTournaments('football');`;

  const pyExample = `import requests

API_BASE_URL = '${API_BASE_URL}'

def get_tournaments(sport=None):
    url = f"{API_BASE_URL}/api/public/events"
    params = {'sport': sport} if sport else {}
    
    try:
        response = requests.get(url, params=params)
        response.raise_for_status()
        
        events = response.json()
        print(f"Знайдено {len(events)} подій.")
        return events
    except requests.exceptions.RequestException as e:
        print(f"Помилка підключення до API: {e}")

get_tournaments('volleyball')`;

  return (
    <div className={styles.apiDocsContainer}>
      <div className={styles.apiDocsHeader}>
        <h1>Публічний API для Розробників</h1>
        <p>Відкритий доступ до спортивних даних Polytech League для ваших проєктів, ботів та аналітики.</p>
      </div>

      <div className={styles.docSection}>
        <h2><i className="fas fa-rocket"></i> Початок роботи</h2>
        <div className={styles.infoBox}>
          <p><strong>Ніяких токенів чи реєстрації.</strong> Даний API є абсолютно відкритим та підтримує CORS (Cross-Origin Resource Sharing). Ви можете робити запити напряму зі свого клієнтського коду (браузера).</p>
        </div>
        <p>Усі запити слід направляти на базову URL-адресу:</p>
        <div className={styles.codeBlock}>
          <code>{API_BASE_URL}</code>
        </div>
      </div>

      <div className={styles.docSection}>
        <h2><i className="fas fa-server"></i> Ендпоінти (Endpoints)</h2>
        
        <div className={styles.endpointCard}>
          <div className={styles.endpointHeader}>
            <span className={styles.methodBadge}>GET</span>
            <span className={styles.endpointUrl}>/api/public/events</span>
          </div>
          <p>Повертає список усіх доступних спортивних подій (турнірів) з базовою інформацією.</p>
          
          <h3>Параметри запиту (Query)</h3>
          <table className={styles.paramsTable}>
            <thead>
              <tr>
                <th>Параметр</th>
                <th>Тип</th>
                <th>Опис</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td><code>sport</code></td>
                <td>string (опціонально)</td>
                <td>Фільтрація за видом спорту (наприклад: <code>football</code>, <code>volleyball</code>)</td>
              </tr>
            </tbody>
          </table>

          <h3>Приклад відповіді (JSON)</h3>
          <div className={styles.codeBlock}>
            <pre>
{`[
  {
    "id": 1,
    "title": "Осінній кубок з футболу 2026",
    "sport": "football",
    "status": "upcoming",
    "date": "15.09.2026",
    "location": "Головний стадіон",
    "description": "Щорічний турнір...",
    "level": "CollegeGroup",
    "category": "Men",
    "participants": 0,
    "teams": 8,
    "isTeam": true,
    "mainPhotoUrl": "/images/cup.jpg"
  }
]`}
            </pre>
          </div>
        </div>

        <div className={styles.endpointCard}>
          <div className={styles.endpointHeader}>
            <span className={styles.methodBadge}>GET</span>
            <span className={styles.endpointUrl}>/api/public/events/{"{id}"}</span>
          </div>
          <p>Повертає вичерпну інформацію про конкретну подію за її ідентифікатором.</p>
          
          <h3>Параметри шляху (Path)</h3>
          <table className={styles.paramsTable}>
            <thead>
              <tr>
                <th>Параметр</th>
                <th>Тип</th>
                <th>Опис</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td><code>id</code></td>
                <td>number (обов'язково)</td>
                <td>Унікальний ідентифікатор події</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div className={styles.docSection}>
        <h2><i className="fas fa-code"></i> Приклади коду</h2>
        
        <h3>JavaScript (Fetch API)</h3>
        <div className={styles.codeBlock}>
          <button className={styles.copyBtn} onClick={() => handleCopy(jsExample, 'js')}>
            <i className={copied === 'js' ? "fas fa-check" : "far fa-copy"}></i> {copied === 'js' ? 'Скопійовано' : 'Копіювати'}
          </button>
          <pre><code>{jsExample}</code></pre>
        </div>

        <h3>Python (Requests)</h3>
        <div className={styles.codeBlock}>
          <button className={styles.copyBtn} onClick={() => handleCopy(pyExample, 'py')}>
            <i className={copied === 'py' ? "fas fa-check" : "far fa-copy"}></i> {copied === 'py' ? 'Скопійовано' : 'Копіювати'}
          </button>
          <pre><code>{pyExample}</code></pre>
        </div>
      </div>
    </div>
  );
};
