/**
 * Utility to handle PWA Push Notifications subscription
 */

const PUBLIC_VAPID_KEY = 'BHAMlRWGwNmfo6Ruf65Idm8TkT9U0H41N6-PXxxqfT9x11GlET16XjgRYmHMb8PVNFs1y321b6ohh1k7HPRnT4w';

export async function requestNotificationPermission() {
  const permission = await Notification.requestPermission();
  if (permission !== 'granted') {
    throw new Error('Permission not granted for notifications');
  }
  return permission;
}

export async function subscribeUserToPush() {
  try {
    if (!('serviceWorker' in navigator)) {
      console.warn('Service Worker is not supported in this browser');
      return null;
    }
    if (!('PushManager' in window)) {
      console.warn('Push Notifications are not supported in this browser');
      return null;
    }

    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready for push subscription');
    
    // Check for existing subscription
    const existingSubscription = await registration.pushManager.getSubscription();
    if (existingSubscription) {
      console.log('Existing push subscription found');
    }
    
    const subscribeOptions = {
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(PUBLIC_VAPID_KEY)
    };

    const subscription = await registration.pushManager.subscribe(subscribeOptions);
    console.log('User is subscribed to Push:', subscription);
    
    // Send subscription to backend
    await sendSubscriptionToBackend(subscription);
    
    return subscription;
  } catch (error) {
    console.error('Failed to subscribe the user to push: ', error);
    throw error;
  }
}

async function sendSubscriptionToBackend(subscription: PushSubscription) {
  const response = await fetch(`${import.meta.env.VITE_API_URL}/api/notifications/subscribe`, {
    method: 'POST',
    body: JSON.stringify(subscription),
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${localStorage.getItem('token')}` // Assuming JWT is stored here
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
