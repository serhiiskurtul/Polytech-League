/**
 * Utility to handle PWA Push Notifications subscription
 */

const PUBLIC_VAPID_KEY = 'BB47cRWxaTwglHKmdQMGkRTn1bMJvD2GGG7a8wr5eEpdCi-KMABJXIUdjG0URYBEcb8PWOcQudBEs9FlsCrsYi0';

export async function requestNotificationPermission() {
  const permission = await Notification.requestPermission();
  if (permission !== 'granted') {
    throw new Error('Permission not granted for notifications');
  }
  return permission;
}

export async function subscribeUserToPush() {
  try {
    if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
      console.warn('Push Notifications are not supported in this browser');
      return null;
    }

    // Register service worker if not already registered (using Vite PWA auto-generated paths)
    let registration = await navigator.serviceWorker.getRegistration();
    if (!registration) {
      const swPath = import.meta.env.DEV ? '/dev-sw.js?dev-sw' : '/sw.js';
      registration = await navigator.serviceWorker.register(swPath, {
        scope: '/'
      });
    }

    // Wait for the service worker to be ready
    await navigator.serviceWorker.ready;

    // Request permission explicitly
    const permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      console.warn('Notification permission not granted');
      return null;
    }

    const subscribeOptions = {
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(PUBLIC_VAPID_KEY)
    };

    const subscription = await registration.pushManager.subscribe(subscribeOptions);
    console.log('Push Subscription generated:', JSON.stringify(subscription));

    // Send subscription to backend
    await sendSubscriptionToBackend(subscription);
    console.log('Subscription successfully sent to backend!');

    return subscription;
  } catch (error) {
    console.error('ПОМИЛКА ПРИ ПІДПИСЦІ:', error);
    // Log more details if it's a fetch error
    if (error instanceof Error) {
      console.error('Error message:', error.message);
    }
    throw error;
  }
}

async function sendSubscriptionToBackend(subscription: PushSubscription) {
  // Use VITE_API_BASE_URL or fallback to production URL
  const apiUrl = import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app';
  // Ensure we use HTTPS for production to avoid Mixed Content issues
  const secureApiUrl = apiUrl.replace('http://', 'https://');
  const url = `${secureApiUrl.endsWith('/') ? secureApiUrl.slice(0, -1) : secureApiUrl}/api/notifications/subscribe`;

  console.log('Sending subscription to:', url);
  alert('Спроба відправити підписку на сервер...');

  const token = localStorage.getItem('token');
  if (!token) {
    throw new Error('User token not found. Please log in again.');
  }

  const response = await fetch(url, {
    method: 'POST',
    body: JSON.stringify(subscription.toJSON()),
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    }
  });

  if (!response.ok) {
    const errorBody = await response.text();
    alert('Помилка сервера при підписці: ' + response.status);
    throw new Error(`Server error (${response.status}): ${errorBody || 'Unknown error'}`);
  }
  alert('Підписка успішно додана в БД!');
}

function urlBase64ToUint8Array(base64String: string) {
  const padding = '='.repeat((4 - base64String.length % 4) % 4);
  const base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/');
  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);
  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}
