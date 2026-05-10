self.addEventListener('push', function (event) {
  let data = { title: 'Polytech League', body: 'Нове сповіщення!' };
  
  if (event.data) {
    try {
      data = event.data.json();
    } catch (e) {
      data = { title: 'Polytech League', body: event.data.text() };
    }
  }

  const options = {
    body: data.body,
    icon: '/pwa-icon-512.png',
    badge: '/favicon.svg',
    vibrate: [100, 50, 100],
    data: {
      url: data.url || '/'
    }
  };

  event.waitUntil(
    self.registration.showNotification(data.title, options)
  );
});

self.addEventListener('notificationclick', function (event) {
  event.notification.close();
  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true }).then(function (clientList) {
      if (clientList.length > 0) {
        let client = clientList[0];
        for (let i = 0; i < clientList.length; i++) {
          if (clientList[i].focused) {
            client = clientList[i];
          }
        }
        return client.focus();
      }
      return clients.openWindow(event.notification.data.url);
    })
  );
});
self.addEventListener('pushsubscriptionchange', function (event) {
  event.waitUntil(
    self.registration.pushManager.subscribe(event.oldSubscription.options)
      .then(function (newSubscription) {
        // In a real app, you would send the new subscription to your server
        console.log('Push subscription changed, new subscription:', newSubscription);
        // We can't easily call our API here without more setup, 
        // but the next time the user opens the app, AuthContext will re-subscribe them.
      })
  );
});
