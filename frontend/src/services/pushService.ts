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
    const registration = await navigator.serviceWorker.ready;
    
    // Check for existing subscription
    const existingSubscription = await registration.pushManager.getSubscription();
    if (existingSubscription) {
      return existingSubscription;
    }

    const subscribeOptions = {
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(PUBLIC_VAPID_KEY)
    };

    const subscription = await registration.pushManager.subscribe(subscribeOptions);
    console.log('User is subscribed:', subscription);
    
    // Send subscription to backend
    await sendSubscriptionToBackend(subscription);
    
    return subscription;
  } catch (error) {
    console.error('Failed to subscribe the user: ', error);
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
