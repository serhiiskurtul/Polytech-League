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
      return null;
    }

    // Register service worker if not already registered
    let registration = await navigator.serviceWorker.getRegistration();
    if (!registration) {
      const swPath = import.meta.env.DEV ? '/dev-sw.js?dev-sw' : '/sw.js';
      registration = await navigator.serviceWorker.register(swPath, {
        scope: '/'
      });
    }

    await navigator.serviceWorker.ready;

    // Request permission explicitly
    const permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      return null;
    }

    const subscribeOptions = {
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(PUBLIC_VAPID_KEY)
    };

    const subscription = await registration.pushManager.subscribe(subscribeOptions);

    // Send subscription to backend
    await sendSubscriptionToBackend(subscription);

    return subscription;
  } catch (error) {
    console.error('Failed to subscribe the user to push: ', error);
    throw error;
  }
}

async function sendSubscriptionToBackend(subscription: PushSubscription) {
  // Use VITE_API_BASE_URL or fallback to production URL
  const apiUrl = import.meta.env.VITE_API_BASE_URL || 'https://polytech-league-production.up.railway.app';
  const url = `${apiUrl.endsWith('/') ? apiUrl.slice(0, -1) : apiUrl}/api/notifications/subscribe`;

  const response = await fetch(url, {
    method: 'POST',
    body: JSON.stringify(subscription.toJSON()),
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}`
    }
  });

  if (!response.ok) {
    throw new Error('Failed to send subscription to backend');
  }
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
